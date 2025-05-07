using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Models
{
    public class PropertyBaseData
    {
        public string PropertyNumber { get; set; } // למשל מס' נכס
        public string Neighborhood { get; set; } // שכונה
        public string Street { get; set; }       // רחוב
        public string HouseNumber { get; set; }  // מספר בית
        public string ApartmentNumber { get; set; } // דירה (אם רלוונטי)
        public double AreaInSquareMeters { get; set; }
        public int SocioEconomicLevel { get; set; }
    }
}
