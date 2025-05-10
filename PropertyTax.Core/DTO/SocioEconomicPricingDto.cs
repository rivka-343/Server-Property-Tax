using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.DTO
{
    public class SocioEconomicPricingDto
    {
        public int Id { get; set; }
        public int SocioEconomicLevel { get; set; }
        public double PricePerSquareMeter { get; set; }
    }
}
