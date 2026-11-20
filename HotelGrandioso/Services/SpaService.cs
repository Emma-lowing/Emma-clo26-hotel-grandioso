using HotelGrandioso.Models;
using HotelGrandioso.Repositories;

namespace HotelGrandioso.Services
{
    internal class SpaService
    {
        private readonly HotelRepository repository;

        public SpaService(HotelRepository repository) // Tar emot och sparar det Repository som spabokningarna ska använda.
        {
            this.repository = repository;
        }

        public void BookSpa() // Låter gästen välja en eller flera spabehandlingar.
        {
            List<Treatment> treatments = repository.GetAllTreatments();
            List<Treatment> selectedTreatments = new List<Treatment>();

            Console.WriteLine("\n=== Spa Grandioso ===");

            while (true)
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
        }

        private Treatment? SelectTreatment(List<Treatment> treatments) // Läser användarens val och kontrollerar att behandlingen finns.
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
    }
}