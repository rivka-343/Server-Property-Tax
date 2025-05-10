using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Models
{
    public class IncomeDiscountTier
    {
        public int Id { get; set; }

        [Required]
        public double MaxIncome { get; set; }  // הכנסה מקסימלית למדרגה זו

        [Required]
        [Range(0, 100)]
        public double DiscountPercentage { get; set; }  // אחוז הנחה
    }
}
