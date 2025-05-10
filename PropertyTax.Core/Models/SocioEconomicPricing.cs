using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Models
{
    // מודל להגדרות מחיר למ"ר לפי רמה סוציו-אקונומית
    public class SocioEconomicPricing
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 10)]
        public int SocioEconomicLevel { get; set; }  // רמה סוציו-אקונומית

        [Required]
        [Range(0, 1000)]
        public double PricePerSquareMeter { get; set; }  // מחיר למ"ר
    }
}
