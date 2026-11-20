
using HotelGrandioso.Data;
using HotelGrandioso.Models;
using HotelGrandioso.Repositories;
using HotelGrandioso.Services;



using var db = new HotelContext(); // Skapar en databaskontext som ger oss tillgång till databasen.

HotelSeeder.Seed(db); // Lägger till hotellets rumstyper och rum om de inte redan finns.

HotelRepository repository = new HotelRepository(db); // Skapar ett Repository som använder vår databaskontext.
BookingService bookingService = new BookingService(repository);
SpaService spaService = new SpaService(repository);



while (true) // Fortsätter visa menyn tills användaren avslutar.
{
    Console.WriteLine("\n=== Hotel Grandioso ===");
    Console.WriteLine("1. Boka rum");
    Console.WriteLine("2. Avboka rum");
    Console.WriteLine("3. Visa bokningar");
    Console.WriteLine("4. Visa alla rum");
    Console.WriteLine("5. Boka spa");
    Console.WriteLine("6. Visa spabokningar");
    Console.WriteLine("0. Avsluta");

    Console.Write("Välj ett alternativ: ");
    string? choice = Console.ReadLine(); 
    switch (choice)
    {
        case "1":
            bookingService.BookRoom();
            break;

        case "2":
            bookingService.CancelRoomBooking();
            break;

        case "3":
            List<Booking> allaBokningar = repository.GetAllBookings(); // Hämtar alla sparade bokningar från databasen.

            foreach (Booking b in allaBokningar) // Går igenom varje bokning i listan.
            {
                decimal totalPrice = repository.CalculateBookingPrice(b);
                Console.WriteLine($"Bokning {b.Id}: {b.Guest.Name}, rum {b.Room.RoomNumber}, {b.CheckIn:yyyy-MM-dd} till {b.CheckOut:yyyy-MM-dd}, totalt {totalPrice} kr");
            }
            break;

        case "4":
            List<Room> allaRum = repository.GetAllRooms(); // Hämtar alla hotellrum från databasen.

            foreach (Room room in allaRum) // Går igenom varje rum i listan.
            {
                Console.WriteLine($"Rum {room.RoomNumber} - {room.RoomType.Name} - {room.PricePerNight} kr/natt");
            }
            break;

        case "5":
            spaService.BookSpa();
            break;
        case "6":
            spaService.ShowSpaBookings();
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Ogiltigt val, försök igen.");
            break;
    }
}


















