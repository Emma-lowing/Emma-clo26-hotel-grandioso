// Startpunkt för Hotel Grandioso.
// Här kopplar du ihop databasen och menyn.

// TODO: Skapa databasen med EF Core (se Data/README.md)

// TODO: Starta menyn (Boka rum, Avboka, Visa bokningar, Visa lediga rum, Avsluta)

using System.ComponentModel.Design;
using HotelGrandioso.Data;
using HotelGrandioso.Models;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== Hotel Grandioso ===");


using var db = new HotelContext();
//Console.WriteLine(System.IO.Path.GetFullPath("hotel.db"));
//Console.WriteLine(db.Database.GetDbConnection().DataSource);



RoomType standard = new RoomType();
standard.Name = "Standard";
standard.Description = "Standardrum";

//db.RoomTypes.Add(standard);
//db.SaveChanges();

Console.WriteLine(db.RoomTypes.Count());
