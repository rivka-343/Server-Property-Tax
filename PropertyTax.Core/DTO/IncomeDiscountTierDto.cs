using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.DTO
{
     public class IncomeDiscountTierDto
    {
        public int Id { get; set; }
        public double MaxIncome { get; set; }
        public double DiscountPercentage { get; set; }
    }
}
