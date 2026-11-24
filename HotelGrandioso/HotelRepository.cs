using HotelGrandioso.Data;
using HotelGrandioso.Models;
using Microsoft.EntityFrameworkCore;


namespace HotelGrandioso.Repositories
{

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

        //Metod GetAllRooms 
        public List<Room> GetAllRooms()
        {
            return db.Rooms
                .Include(r => r.RoomType)
                .ToList();
        }


        // Metod AddBooking som sparar en ny bokning i databasen.
        public BookingResult AddBooking(Booking booking)
        {
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

        //Metod AddGuest som sparar en ny gäst i databasen.
        public void AddGuest(Guest guest)
        {
            db.Guests.Add(guest);
            db.SaveChanges();
        }

        // Hämtar alla bokningar med tillhörande gäst och rum.
        public List<Booking> GetAllBookings()
        {
            return db.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }
        // Metod tar bort en bokning från databasen med hjälp av dess ID.
        public bool CancelBooking(int bookingId)
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

        public decimal CalculateBookingPrice(Booking booking)
        {
            Room room = db.Rooms.First(r => r.Id == booking.RoomId);
            int nights = (booking.CheckOut - booking.CheckIn).Days;
            return nights * room.PricePerNight;
        }

        // Metod som visar tillgängliga rum 
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
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
        public List<Treatment> GetAllTreatments() // Hämtar alla spabehandlingar från databasen.
        {
            return db.Treatments.ToList();
        }

        public int AddSpaBooking(SpaBooking spaBooking)
        {
            db.SpaBookings.Add(spaBooking);
            db.SaveChanges();

            return spaBooking.Id;
        }

        public List<SpaBooking> GetAllSpaBookings() // Hämtar alla spabokningar tillsammans med gäst och behandling.
        {
            return db.SpaBookings
                .Include(s => s.Guest)
                .Include(s => s.Treatment)
                .ToList();
        }
    }
}