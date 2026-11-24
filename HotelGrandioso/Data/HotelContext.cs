using System;
using System.Collections.Generic;
using System.Text;
using HotelGrandioso.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelGrandioso.Data
{
    public class HotelContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Guest> Guests { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<RoomType> RoomTypes { get; set; } = null!;
        public DbSet<SpaBooking> SpaBookings { get; set; } = null!;
        public DbSet<Treatment> Treatments { get; set; } = null!;


        protected override void
       
        OnConfiguring(DbContextOptionsBuilder options)
        {
            // Använder en miljövariabel för att kunna ändra databasanslutningen utan att ändra källkoden.
            string? connectionString = Environment.GetEnvironmentVariable("HOTEL_DB");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Miljövariabeln HOTEL_DB saknas.");
            }
            options.UseSqlite(connectionString);
        }
    }
}
 