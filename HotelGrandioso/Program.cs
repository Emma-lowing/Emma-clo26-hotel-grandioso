// Startpunkt för Hotel Grandioso.
// Här kopplar du ihop databasen och menyn.

// TODO: Skapa databasen med EF Core (se Data/README.md)

// TODO: Starta menyn (Boka rum, Avboka, Visa bokningar, Visa lediga rum, Avsluta)


using HotelGrandioso.Data;
using HotelGrandioso.Models;


Console.WriteLine("=== Hotel Grandioso ===");


using var db = new HotelContext();



RoomType standard = new RoomType();
standard.Name = "Standard";
standard.Description = "Standardrum";

RoomType lyxsvit = new RoomType();
lyxsvit.Name = "Lyxsvit";
lyxsvit.Description = "Lyxigt hotellrum";

RoomType brollopsvit = new RoomType();
brollopsvit.Name = "Bröllopsvit";
brollopsvit.Description = "Bröllopsvit";


List<RoomType> roomtype = new List<RoomType>
{
    standard,
    lyxsvit,
    brollopsvit
};

foreach (RoomType type in roomtype)
{
    if (!db.RoomTypes.Any(r => r.Name == type.Name)) //Fick ai hjälp att formulera if-satsen.
    {
        db.RoomTypes.Add(type);

    }
    else
    {
        Console.WriteLine($"{type.Name} finns redan");
    }
}
db.SaveChanges();


Console.WriteLine(db.RoomTypes.Count());


int standardId = db.RoomTypes.First(r => r.Name == "Standard").Id;
int lyxsvitId = db.RoomTypes.First(r => r.Name == "Lyxsvit").Id;
int brollopsvitId = db.RoomTypes.First(r => r.Name == "Bröllopsvit").Id;

Room room101 = new Room();
room101.RoomNumber = 101;
room101.Floor = 1;
room101.SizeM2 = 80;
room101.PricePerNight = 800;
room101.RoomTypeId = standardId;

Room room102 = new Room();
room102.RoomNumber = 102;
room102.Floor = 1;
room102.SizeM2 = 80;
room102.PricePerNight = 800;
room102.RoomTypeId = standardId;

Room room103 = new Room();
room103.RoomNumber = 103;
room103.Floor = 1;
room103.SizeM2 = 80;
room103.PricePerNight = 800;
room103.RoomTypeId = standardId;

Room room104 = new Room();
room104.RoomNumber = 104;
room104.Floor = 1;
room104.SizeM2 = 80;
room104.PricePerNight = 800;
room104.RoomTypeId = standardId;

Room room105 = new Room();
room105.RoomNumber = 105;
room105.Floor = 1;
room105.SizeM2 = 80;
room105.PricePerNight = 800;
room105.RoomTypeId = standardId;

Room room201 = new Room();
room201.RoomNumber = 201;
room201.Floor = 2;
room201.SizeM2 = 100;
room201.PricePerNight = 1000;
room201.RoomTypeId = standardId;

Room room202 = new Room();
room202.RoomNumber = 202;
room202.Floor = 2;
room202.SizeM2 = 100;
room202.PricePerNight = 1000;
room202.RoomTypeId = standardId;

Room room203 = new Room();
room203.RoomNumber = 203;
room203.Floor = 2;
room203.SizeM2 = 100;
room203.PricePerNight = 1000;
room203.RoomTypeId = standardId;

Room room204 = new Room();
room204.RoomNumber = 204;
room204.Floor = 2;
room204.SizeM2 = 100;
room204.PricePerNight = 1000;
room204.RoomTypeId = standardId;

Room room205 = new Room();
room205.RoomNumber = 205;
room205.Floor = 2;
room205.SizeM2 = 100;
room205.PricePerNight = 1000;
room205.RoomTypeId = standardId;

Room room301 = new Room();
room301.RoomNumber = 301;
room301.Floor = 3;
room301.SizeM2 = 120;
room301.PricePerNight = 1200;
room301.RoomTypeId = standardId;

Room room302 = new Room();
room302.RoomNumber = 302;
room302.Floor = 3;
room302.SizeM2 = 120;
room302.PricePerNight = 1200;
room302.RoomTypeId = standardId;

Room room303 = new Room();
room303.RoomNumber = 303;
room303.Floor = 3;
room303.SizeM2 = 120;
room303.PricePerNight = 1200;
room303.RoomTypeId = standardId;

Room room304 = new Room();
room304.RoomNumber = 304;
room304.Floor = 3;
room304.SizeM2 = 180;
room304.PricePerNight = 2800;
room304.RoomTypeId = lyxsvitId;

Room room305 = new Room();
room305.RoomNumber = 305;
room305.Floor = 3;
room305.SizeM2 = 220;
room305.PricePerNight = 3500;
room305.RoomTypeId = brollopsvitId;


List<Room> rooms = new List<Room>
{
    room101,
    room102,
    room103,
    room104,
    room105,
    room201,
    room202,
    room203,
    room204,
    room205,
    room301,
    room302,
    room303,
    room304,
    room305
};

foreach (Room room in rooms)
{
    if (!db.Rooms.Any(r=>r.RoomNumber == room.RoomNumber))
    {
        db.Rooms.Add(room);
    }
    else
    {
        Console.WriteLine($"Rum {room.RoomNumber} finns redan");
    }
}

db.SaveChanges();

Console.WriteLine($"Antal rum: {db.Rooms.Count()}");