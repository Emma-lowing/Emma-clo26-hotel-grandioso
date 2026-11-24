using HotelGrandioso.Models;
using HotelGrandioso.Repositories;

namespace HotelGrandioso.Services
{
    internal class SpaService
    {
        private readonly HotelRepository repository;

        public SpaService(HotelRepository repository) // Tar emot Repository utifrån för att hålla databasarbetet separerat från spabokningsflödet.
        {
            this.repository = repository;
        }

        public void BookSpa() 
        {
            List<Treatment> treatments = repository.GetAllTreatments();
            List<Treatment> selectedTreatments = new List<Treatment>();

            Console.WriteLine("\n=== Spa Grandioso ===");

            while (true) // Använder en loop för att gästen ska kunna välja flera behandlingar vid samma bokningstillfälle.
            {
                Console.WriteLine("\nTillgängliga behandlingar:");

                foreach (Treatment treatment in treatments)
                {
                    Console.WriteLine($"{treatment.Id}. {treatment.Name} - {treatment.Price} kr");
                }

                Treatment? selectedTreatment = SelectTreatment(treatments);

                if (selectedTreatment == null)
                    return;

                selectedTreatments.Add(selectedTreatment);

                Console.WriteLine($"Tillagd behandling: {selectedTreatment.Name}");

                Console.Write("Vill du lägga till en behandling till? (J/N): ");
                string? answer = Console.ReadLine();

                if (answer?.Trim().ToUpper() != "J")
                    break;
            }

            decimal totalPrice = selectedTreatments.Sum(t => t.Price);

            Console.WriteLine("\n=== Valda behandlingar ===");

            foreach (Treatment treatment in selectedTreatments)
            {
                Console.WriteLine($"{treatment.Name} - {treatment.Price} kr");
            }

            Console.WriteLine($"Totalt pris: {totalPrice} kr");

            DateOnly? spaDate = ReadSpaDate();
            if (spaDate == null)
                return;
            string? guestName = ReadGuestName();
            if (guestName == null)
                return;
            Console.WriteLine("\n=== Sammanfattning ===");
            Console.WriteLine($"Gäst: {guestName}");
            Console.WriteLine($"Datum: {spaDate:yyyy-MM-dd}");
            Console.WriteLine($"Antal behandlingar: {selectedTreatments.Count}");
            Console.WriteLine($"Totalpris: {totalPrice} kr");
            Console.Write("Bekräfta spabokningen (J/N): ");
            string? confirmation = Console.ReadLine();

            if (confirmation?.Trim().ToUpper() != "J")
            {
                Console.WriteLine("Spabokningen avbröts.");
                return;
            }
            Guest guest = new Guest(); // Skapar gästen utanför loopen så att alla valda behandlingar kopplas till samma gäst.
            guest.Name = guestName;


            foreach (Treatment treatment in selectedTreatments) // Sparar varje behandling som en egen spabokning så att flera behandlingar kan kopplas till samma gäst.
            {
                SpaBooking spaBooking = new SpaBooking();

                spaBooking.Guest = guest;
                spaBooking.TreatmentId = treatment.Id;
                spaBooking.Date = spaDate.Value.ToDateTime(TimeOnly.MinValue);

                int bookingId = repository.AddSpaBooking(spaBooking);
                Console.WriteLine($"Spabokning skapad med ID: {bookingId}");
            }

            Console.WriteLine("Alla spabehandlingar har bokats!");
        }

        private Treatment? SelectTreatment(List<Treatment> treatments) // Kontrollerar att behandlingen finns för att förhindra bokning av en ogiltig behandling.
        {
            Console.Write("Välj behandlingens nummer: ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int treatmentId))
            {
                Console.WriteLine("Ogiltigt nummer.");
                return null;
            }

            Treatment? selectedTreatment = treatments.FirstOrDefault(t => t.Id == treatmentId);

            if (selectedTreatment == null)
            {
                Console.WriteLine("Behandlingen finns inte.");
                return null;
            }

            return selectedTreatment;
        }

        private DateOnly? ReadSpaDate() // Validerar datumet för att förhindra bokningar med ogiltiga eller passerade datum. 
        {
            Console.Write("Ange datum för spabesöket (ÅÅÅÅ-MM-DD): ");
            string? input = Console.ReadLine();

            if (!DateOnly.TryParseExact(input, "yyyy-MM-dd", out DateOnly date))
            {
                Console.WriteLine("Ogiltigt datum.");
                return null;
            }

            if (date < DateOnly.FromDateTime(DateTime.Today))
            {
                Console.WriteLine("Du kan inte boka ett datum som redan har passerat.");
                return null;
            }

            return date;
        }

        private string? ReadGuestName() // Förhindrar att spabokningar skapas utan ett angivet gästnamn.
        {
            Console.Write("Ange gästens namn: ");
            string? guestName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(guestName))
            {
                Console.WriteLine("Du måste ange ett namn.");
                return null;
            }

            return guestName.Trim();
        }

        public void ShowSpaBookings()
        {
            List<SpaBooking> spaBookings = repository.GetAllSpaBookings();
            Console.WriteLine("\n=== Spabokningar ===");
            if (spaBookings.Count == 0)
            {
                Console.WriteLine("Det finns inga spabokningar.");
                return;
            }
            foreach (SpaBooking booking in spaBookings)
            {
                Console.WriteLine($"\nBoknings-ID: {booking.Id}");
                Console.WriteLine($"Gäst: {booking.Guest.Name}");
                Console.WriteLine($"Datum: {booking.Date:yyyy-MM-dd}");
                Console.WriteLine($"Behandling: {booking.Treatment.Name}");
                Console.WriteLine($"Pris: {booking.Treatment.Price} kr");
            }
        }

    }
}