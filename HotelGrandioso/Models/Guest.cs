using System;
using System.Collections.Generic;
using System.Text;

namespace HotelGrandioso.Models
{
    // Sparar endast ID och namn för att följa GDPR:s princip om dataminimering.
    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

    }
}
