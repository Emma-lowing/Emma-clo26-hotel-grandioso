# Hotel Grandioso — Inlämning 1

**Namn:** Emma Löwing

## Så kör du projektet

För att kunna köra projektet behöver du ha .NET 10 SDK installerat.
1. Forka repot på GitHub och klona din fork.
2. Öppna `HotelGrandioso.slnx` i Visual Studio (eller öppna mappen i VS Code).
3. Ange miljövariabeln `HOTEL_DB` innan programmet startas. Den används för att ansluta till SQLite-databasen.

   I PowerShell skriver du:

   ```powershell
   $env:HOTEL_DB = "Data Source=hotel.db"
   ```

4. Databasen skapas och uppdateras automatiskt när programmet startas. Du behöver därför inte köra migrationerna manuellt.
5. Starta programmet med **F5** i Visual Studio, förutsatt att miljövariabeln är tillgänglig för programmet.

**Via terminalen:**

Öppna terminalen i projektets rotmapp (där HotelGrandioso.slnx finns). När miljövariabeln har angetts i samma terminal kan programmet startas med:

```bash
dotnet run --project HotelGrandioso
```


## Funktionalitet

Programmet innehåller följande funktioner:

- Boka hotellrum.
- Avboka hotellrum.
- Visa alla rumsbokningar.
- Visa alla hotellrum med rumstyp och pris.
- Visa lediga rum under ett valt datumintervall utan att behöva göra en bokning.
- Boka en eller flera spabehandlingar.
- Visa alla spabokningar.

Programmet beräknar även totalpriset för bokningar och förhindrar dubbelbokningar av hotellrum.

## Databas

Projektet använder SQLite som databas och Entity Framework Core med Code First.

Jag har skapat entiteterna i C# och använt migrationer för att skapa motsvarande tabeller i databasen.

Databasanslutningen anges genom miljövariabeln `HOTEL_DB`.

För att ange anslutningssträngen i PowerShell:

```powershell
$env:HOTEL_DB = "Data Source=hotel.db"
```

Jag använder `db.Database.Migrate()` i `Program.cs` för att automatiskt köra befintliga migrationer när programmet startas. På så sätt skapas databasen och tabellerna även när programmet startas från en ren klon av projektet.

Migrationerna körs innan `HotelSeeder.Seed(db)` anropas för att säkerställa att tabellerna finns innan information läggs till.

Databasen innehåller information om rum, rumstyper, gäster, rumsbokningar, behandlingar och spabokningar.

- **Databas:** SQLite.
- **Migrationer:** Jag använder Entity Framework Core Code First. Jag har skapat den första migrationen i Visual Studios Package Manager Console med `Add-Migration InitialCreate`. Befintliga migrationer körs automatiskt när programmet startas.
  
## Seeding

Jag har skapat klassen `HotelSeeder` för att lägga in hotellets rum, rumstyper och spabehandlingar i databasen.

Jag använder seeding för att slippa lägga in all information manuellt varje gång jag skapar en ny databas.

Jag använder också `Any()` för att kontrollera om informationen redan finns, så att samma rum och behandlingar inte läggs till flera gånger när programmet startas.


## UML-klassdiagram


<img width="2400" height="1570" alt="hotel-grandioso2-uml" src="https://github.com/user-attachments/assets/ec657300-077f-4ff1-a6da-e8e3d70f6f51" />


## GDPR

När jag skapade entiteten Guest valde jag att bara spara Id och Name. Jag utgick ifrån att systemet inte ska samla in fler personuppgifter än vad som behövs för att kunna hantera bokningarna.

**Id:** Jag använder ett unikt ID för att kunna skilja gäster åt, även om två gäster har samma namn.

**Name:** Jag sparar gästens namn för att kunna se vem som har gjort en bokning.

**HairColor:** Jag valde att inte spara hårfärg eftersom det inte behövs för att boka ett hotellrum eller en spabehandling.

**HeightCm:** Jag valde att inte spara längd eftersom det inte är relevant för bokningssystemet.

**Religion:** Jag valde att inte spara religion eftersom det inte behövs för bokningen. Religion är dessutom en känslig personuppgift enligt GDPR.

Mitt mål har varit att följa principen om dataminimering, alltså att bara samla in de personuppgifter som behövs för systemets syfte.


## Designmönster

Jag har använt Repository-mönstret genom att skapa klassen `HotelRepository`.

`HotelRepository` ansvarar för att hämta, skapa och ta bort information i databasen, exempelvis bokningar, rum och spabokningar.

Jag valde att separera databasarbetet från resten av programmet för att göra koden tydligare och lättare att underhålla. På så sätt behöver jag inte skriva databasfrågor direkt i `Program.cs`, `BookingService.cs` eller `SpaService.cs`.

`BookingService` och `SpaService` ansvarar för bokningsprocesserna och kommunikationen med användaren, medan `HotelRepository` ansvarar för databasarbetet.

För att behålla denna ansvarsfördelning returnerar `HotelRepository` resultat istället för att skriva ut meddelanden i konsolen. Vid rumsbokning använder jag en enum, `BookingResult`, för att skilja mellan en lyckad bokning, ett redan bokat rum och ogiltiga datum. Vid spabokning returneras istället bokningens ID.

På så sätt behöver `HotelRepository` inte känna till hur informationen presenteras för användaren.



## Tekniker

- **C# / .NET 10** – Programmeringsspråk och ramverk.
- **Entity Framework Core** – Används för att arbeta med databasen genom C#-klasser.
- **SQLite** – Databasen där rum, gäster och bokningar sparas.
- **Code First och migrationer** – Används för att skapa och uppdatera databasen utifrån entiteterna.
- **LINQ** – Används för att söka, filtrera och hämta information från databasen.
- **Repository-mönstret** – Samlar databasarbetet i klassen `HotelRepository`.
