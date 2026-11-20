
using HotelGrandioso.Models;
using HotelGrandioso.Repositories;

namespace HotelGrandioso.Services
{
    internal class BookingService
    {
        private readonly HotelRepository repository;

        public BookingService(HotelRepository repository) // Tar emot och sparar det Repository som bokningstjänsten ska använda.
        {
            this.repository = repository;
        }

        public void BookRoom() // Håller ihop hela bokningsprocessen och anropar de mindre metoderna.
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

            repository.AddBooking(booking);
        }

        private (DateTime checkIn, DateTime checkOut)? ReadDates() // Läser in och kontrollerar in- och utcheckningsdatum.
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

            if (checkOut <= checkIn)
            {
                Console.WriteLine("Utcheckningsdatum måste vara senare än incheckningsdatum.");
                return null;
            }

            return (checkIn, checkOut);
        }

        private void ShowAvailableRooms(List<Room> availableRooms, DateTime checkIn, DateTime checkOut) // Visar lediga rum med storlek, pris per natt och totalpris.
        {
            Console.WriteLine("\nLediga rum:");

            int nights = (checkOut - checkIn).Days;

            foreach (Room room in availableRooms)
            {
                decimal totalPrice = nights * room.PricePerNight;

                Console.WriteLine($"Rum {room.RoomNumber} - {room.RoomType.Name} - {room.SizeM2} m² - {room.PricePerNight} kr/natt - Totalt {totalPrice} kr");
            }
        }

        private Room? SelectRoom(List<Room> availableRooms) // Låter användaren välja ett rum och kontrollerar att det finns bland de lediga rummen.
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

        private string? ReadGuestName() // Läser in gästens namn och kontrollerar att det inte är tomt.
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

        private bool ConfirmBooking(Booking booking, Room selectedRoom) // Visar bokningssammanfattningen och frågar om användaren vill bekräfta.
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

        public void CancelRoomBooking() // Låter användaren avboka en befintlig bokning.
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
