using PropertyTax.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Services
{

    public interface IDiscountSettingsService
    {
        Task<SystemSettingsDto> GetAllSettingsAsync();
        Task<bool> UpdateSettingsAsync(SystemSettingsDto settingsDto, string username);
        Task<double> CalculateDiscountPercentageAsync(double averageMonthlyIncome);
        Task<double> GetPricePerSquareMeterAsync(int socioEconomicLevel);
    }
}
