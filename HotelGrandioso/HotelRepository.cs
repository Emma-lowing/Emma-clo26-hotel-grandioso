using HotelGrandioso.Data;
using HotelGrandioso.Models;
using Microsoft.EntityFrameworkCore;


namespace HotelGrandioso.Repositories
{

    // Använder en enum för att skilja mellan olika bokningsresultat
    // utan att Repository behöver hantera meddelanden till användaren.
    public enum BookingResult
    {
        Success,
        AlreadyBooked,
        InvalidDates
    }
    internal class HotelRepository
    {
        private readonly HotelContext db;

        public HotelRepository(HotelContext context)
        {
            db = context;
        }

        // Metoder för rum och rumsbokningar

       
        public List<Room> GetAllRooms() // Inkluderar rumstypen så att användargränssnittet kan visa rummets typ.
        {
            return db.Rooms
                .Include(r => r.RoomType)
                .ToList();
        }



        public BookingResult AddBooking(Booking booking)// Kontrollerar tillgängligheten igen innan bokningen sparas för att förhindra dubbelbokningar.
        {
            // Använder strikta jämförelser så att ett rum kan bokas igen samma dag som en annan gäst checkar ut.
            bool isBooked = db.Bookings.Any(existing => existing.RoomId == booking.RoomId && existing.CheckIn < booking.CheckOut && booking.CheckIn < existing.CheckOut);
            if (isBooked)
            {
                return BookingResult.AlreadyBooked;
            }
           
            if (booking.CheckOut <= booking.CheckIn)
            {
                return BookingResult.InvalidDates;
            }
            db.Bookings.Add(booking);
            db.SaveChanges();
            return BookingResult.Success;
        }

        
        public void AddGuest(Guest guest)
        {
            db.Guests.Add(guest);
            db.SaveChanges();
        }

        
        public List<Booking> GetAllBookings()// Inkluderar gäst och rum så att bokningsinformationen kan visas utan separata databasfrågor.
        {
            return db.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }
       
        public bool CancelBooking(int bookingId) // Använder bokningens unika ID för att säkerställa att rätt bokning avbokas.
        {
            Booking? booking = db.Bookings
                .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
            {
                return false;
            }

            db.Bookings.Remove(booking);
            db.SaveChanges();

            return true;
        }

        public decimal CalculateBookingPrice(Booking booking) // Beräknar priset utifrån rummets pris per natt och bokningens längd.
        {
            Room room = db.Rooms.First(r => r.Id == booking.RoomId);
            int nights = (booking.CheckOut - booking.CheckIn).Days;
            return nights * room.PricePerNight;
        }

        
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut) // Utesluter rum med överlappande bokningar för att endast returnera lediga rum.
        {
            return db.Rooms
                .Include(r => r.RoomType)
                .Where(room => !db.Bookings.Any(b =>
                    b.RoomId == room.Id &&
                    b.CheckIn < checkOut &&
                    checkIn < b.CheckOut))
                .ToList();
        }

        // Metoder för spa
        public List<Treatment> GetAllTreatments() 
        {
            return db.Treatments.ToList();
        }

        public int AddSpaBooking(SpaBooking spaBooking) // Returnerar bokningens ID så att användargränssnittet kan visa en bekräftelse.
        {
            db.SpaBookings.Add(spaBooking);
            db.SaveChanges();

            return spaBooking.Id;
        }

        public List<SpaBooking> GetAllSpaBookings() // Inkluderar gäst och behandling så att fullständig bokningsinformation kan visas.
        {
            return db.SpaBookings
                .Include(s => s.Guest)
                .Include(s => s.Treatment)
                .ToList();
        }
    }
}