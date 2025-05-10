using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.DTO
{
    public class SystemSettingsDto
    {
        public List<IncomeDiscountTierDto> IncomeTiers { get; set; }
        public List<SocioEconomicPricingDto> SocioEconomicPrices { get; set; }
        public DateTime LastUpdated { get; set; }
        public string UpdatedBy { get; set; }
    }
}
