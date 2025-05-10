using PropertyTax.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Repositories
{
    public interface IDiscountSettingsRepository
    {
        Task<List<IncomeDiscountTier>> GetIncomeDiscountTiersAsync();
        Task<List<SocioEconomicPricing>> GetSocioEconomicPricingsAsync();
        Task<bool> UpdateIncomeDiscountTiersAsync(List<IncomeDiscountTier> tiers);
        Task<bool> UpdateSocioEconomicPricingsAsync(List<SocioEconomicPricing> pricings);
        Task<SystemSettings> GetSystemSettingsAsync(string settingName);
        Task UpdateSystemSettingsAsync(SystemSettings settings);
    }
}
