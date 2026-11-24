
using HotelGrandioso.Data;
using HotelGrandioso.Models;
using HotelGrandioso.Repositories;
using HotelGrandioso.Services;
using Microsoft.EntityFrameworkCore;



using var db = new HotelContext(); // Använder using för att frigöra databasresurser när programmet avslutas
db.Database.Migrate(); //Kör migrationerna automatiskt så att tabellerna finns även när programmet startas från en ren klon. 
HotelSeeder.Seed(db); // Använder seeding för att hotellets grunddata ska finnas även i en ny databas.

HotelRepository repository = new HotelRepository(db); // Använder samma Repository i båda tjänsterna för att samla databasarbetet på ett ställe.
BookingService bookingService = new BookingService(repository);
SpaService spaService = new SpaService(repository);



while (true) // Fortsätter visa menyn tills användaren avslutar.
{
    Console.WriteLine("\n=== Hotel Grandioso ===");
    Console.WriteLine("1. Boka rum");
    Console.WriteLine("2. Avboka rum");
    Console.WriteLine("3. Visa bokningar");
    Console.WriteLine("4. Visa alla rum");
    Console.WriteLine("5. Visa lediga rum");
    Console.WriteLine("6. Boka spa");
    Console.WriteLine("7. Visa spabokningar");
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

            List<Booking> allaBokningar = repository.GetAllBookings();

            if (allaBokningar.Count == 0)
            {
                Console.WriteLine("Det finns inga sparade rumsbokningar.");
                break;
            }

            foreach (Booking b in allaBokningar) 
            {
                decimal totalPrice = repository.CalculateBookingPrice(b);
                Console.WriteLine($"Bokning {b.Id}: {b.Guest.Name}, rum {b.Room.RoomNumber}, {b.CheckIn:yyyy-MM-dd} till {b.CheckOut:yyyy-MM-dd}, totalt {totalPrice} kr");
            }
            break;

        case "4":
            List<Room> allaRum = repository.GetAllRooms(); 

            foreach (Room room in allaRum) 
            {
                Console.WriteLine($"Rum {room.RoomNumber} - {room.RoomType.Name} - {room.PricePerNight} kr/natt");
            }
            break;
        case "5":
            bookingService.SearchAvailableRooms();
            break;

        case "6":
            spaService.BookSpa();
            break;
        case "7":
            spaService.ShowSpaBookings();
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Ogiltigt val, försök igen.");
            break;
    }
}