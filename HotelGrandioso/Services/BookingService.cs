
using HotelGrandioso.Models;
using HotelGrandioso.Repositories;

namespace HotelGrandioso.Services
{
    internal class BookingService
    {
        private readonly HotelRepository repository;

        public BookingService(HotelRepository repository) // Tar emot Repository utifrån för att hålla databasarbetet separerat från bokningsflödet.
        {
            this.repository = repository;
        }

        public void BookRoom() // Delar upp bokningsprocessen i mindre metoder för att göra koden lättare att förstå och underhålla.
        {
            (DateTime checkIn, DateTime checkOut)? dates = ReadDates();

            if (dates == null)
                return;

            DateTime checkIn = dates.Value.checkIn;
            DateTime checkOut = dates.Value.checkOut;

            List<Room> availableRooms = repository.GetAvailableRooms(checkIn, checkOut);

            if (availableRooms.Count == 0)
            {
                Console.WriteLine("Det finns inga lediga rum under den valda perioden.");
                return;
            }

            ShowAvailableRooms(availableRooms, checkIn, checkOut);

            Room? selectedRoom = SelectRoom(availableRooms);

            if (selectedRoom == null)
                return;

            string? guestName = ReadGuestName();

            if (guestName == null)
                return;

            Guest guest = new Guest();
            guest.Name = guestName;

            Booking booking = new Booking();
            booking.Guest = guest;
            booking.RoomId = selectedRoom.Id;
            booking.CheckIn = checkIn;
            booking.CheckOut = checkOut;

            if (!ConfirmBooking(booking, selectedRoom))
                return;

            BookingResult result = repository.AddBooking(booking);
            switch (result) // Hanterar meddelandena här för att hålla användarinteraktionen separerad från databasarbetet.
            {
                case BookingResult.Success:
                    Console.WriteLine($"Bokning skapad med ID: {booking.Id}");
                    break;

                case BookingResult.AlreadyBooked:
                    Console.WriteLine("Rummet är redan bokat under dessa datum.");
                    break;

                case BookingResult.InvalidDates:
                    Console.WriteLine("Utcheckningsdatum måste vara senare än incheckningsdatum.");
                    break;
            }
        }

        private (DateTime checkIn, DateTime checkOut)? ReadDates() // Validerar datumen för att förhindra bokningar med ogiltiga datumintervall.
        {
            Console.Write("Ange incheckningsdatum (ÅÅÅÅ-MM-DD): ");
            string? checkInInput = Console.ReadLine();

            if (!DateTime.TryParse(checkInInput, out DateTime checkIn))
            {
                Console.WriteLine("Ogiltigt datum.");
                return null;
            }

            Console.Write("Ange utcheckningsdatum (ÅÅÅÅ-MM-DD): ");
            string? checkOutInput = Console.ReadLine();

            if (!DateTime.TryParse(checkOutInput, out DateTime checkOut))
            {
                Console.WriteLine("Ogiltigt datum.");
                return null;
            }
            if (checkIn.Date < DateTime.Today)
            {
                Console.WriteLine("Du kan inte boka ett datum som redan har passerat.");
                return null;
            }
            if (checkOut <= checkIn)
            {
                Console.WriteLine("Utcheckningsdatum måste vara senare än incheckningsdatum.");
                return null;
            }

            return (checkIn, checkOut);
        }

        private void ShowAvailableRooms(List<Room> availableRooms, DateTime checkIn, DateTime checkOut) // Visar även totalpriset så att användaren kan jämföra rummen innan en bokning görs.
        {
            Console.WriteLine("\nLediga rum:");

            int nights = (checkOut - checkIn).Days;

            foreach (Room room in availableRooms)
            {
                decimal totalPrice = nights * room.PricePerNight;

                Console.WriteLine($"Rum {room.RoomNumber} - Våning {room.Floor} - {room.RoomType.Name} - {room.SizeM2} m² - {room.PricePerNight} kr/natt - Totalt {totalPrice} kr");
            }
        }

        private Room? SelectRoom(List<Room> availableRooms) // Begränsar valet till lediga rum för att förhindra att användaren väljer ett upptaget rum.
        {
            Console.Write("Välj rumsnummer: ");
            string? roomInput = Console.ReadLine();

            if (!int.TryParse(roomInput, out int roomNumber))
            {
                Console.WriteLine("Ogiltigt rumsnummer.");
                return null;
            }

            Room? selectedRoom = availableRooms.FirstOrDefault(r => r.RoomNumber == roomNumber);

            if (selectedRoom == null)
            {
                Console.WriteLine("Rummet finns inte bland de lediga rummen.");
                return null;
            }

            return selectedRoom;
        }

        private string? ReadGuestName() // Förhindrar att bokningar skapas utan ett angivet gästnamn.
        {
            Console.Write("Ange gästens namn: ");
            string? guestName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(guestName))
            {
                Console.WriteLine("Du måste ange ett namn.");
                return null;
            }

            return guestName;
        }

        private bool ConfirmBooking(Booking booking, Room selectedRoom) // Låter användaren kontrollera bokningsuppgifterna och totalpriset innan bokningen sparas.
        {
            int nights = (booking.CheckOut - booking.CheckIn).Days;
            decimal totalPrice = nights * selectedRoom.PricePerNight;

            Console.WriteLine("\n=== Bekräfta bokning ===");
            Console.WriteLine($"Gäst: {booking.Guest.Name}");
            Console.WriteLine($"Rum: {selectedRoom.RoomNumber} - {selectedRoom.RoomType.Name}");
            Console.WriteLine($"Incheckning: {booking.CheckIn:yyyy-MM-dd}");
            Console.WriteLine($"Utcheckning: {booking.CheckOut:yyyy-MM-dd}");
            Console.WriteLine($"Antal nätter: {nights}");
            Console.WriteLine($"Totalpris: {totalPrice} kr");

            Console.Write("Bekräfta bokning (J/N): ");
            string? confirmation = Console.ReadLine();

            if (confirmation?.Trim().ToUpper() != "J")
            {
                Console.WriteLine("Bokningen avbröts.");
                return false;
            }

            return true;
        }
        public void SearchAvailableRooms() // Återanvänder befintliga metoder så att användaren kan söka efter lediga rum utan att behöva påbörja en bokning.
        {
            (DateTime checkIn, DateTime checkOut)? dates = ReadDates();

            if (dates == null)
                return;
            DateTime checkIn = dates.Value.checkIn;
            DateTime checkOut = dates.Value.checkOut;
            List<Room> availableRooms = repository.GetAvailableRooms(checkIn, checkOut);
            if (availableRooms.Count == 0)
            {
                Console.WriteLine("Det finns inga lediga rum under den valda perioden.");
                return;
            }
            ShowAvailableRooms(availableRooms, checkIn, checkOut);
        }
        public void CancelRoomBooking() 
        {
            Console.Write("Ange boknings-ID: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int bookingId))
            {
                Console.WriteLine("Ogiltigt boknings-ID.");
                return;
            }

            bool cancelled = repository.CancelBooking(bookingId);

            if (cancelled)
            {
                Console.WriteLine($"Bokning {bookingId} har avbokats.");
            }
            else
            {
                Console.WriteLine("Ingen bokning hittades med det ID-numret.");
            }
        }
    }
}
