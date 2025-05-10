using AutoMapper;
using PropertyTax.Core.DTO;
using PropertyTax.Core.Models;
using PropertyTax.Core.Repositories;
using PropertyTax.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Service
{
    public class DiscountSettingsService : IDiscountSettingsService
    {
        private readonly IDiscountSettingsRepository _repository;
        private readonly IMapper _mapper;

        public DiscountSettingsService(IDiscountSettingsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SystemSettingsDto> GetAllSettingsAsync()
        {
            var incomeTiers = await _repository.GetIncomeDiscountTiersAsync();
            var socioEconomicPrices = await _repository.GetSocioEconomicPricingsAsync();
            var systemSettings = await _repository.GetSystemSettingsAsync("DiscountSettings");

            var result = new SystemSettingsDto
            {
                IncomeTiers = _mapper.Map<List<IncomeDiscountTierDto>>(incomeTiers),
                SocioEconomicPrices = _mapper.Map<List<SocioEconomicPricingDto>>(socioEconomicPrices),
                LastUpdated = systemSettings?.LastUpdated ?? DateTime.Now,
                UpdatedBy = systemSettings?.UpdatedBy ?? "System"
            };

            return result;
        }

        public async Task<bool> UpdateSettingsAsync(SystemSettingsDto settingsDto, string username)
        {
            try
            {
                var incomeTiers = _mapper.Map<List<IncomeDiscountTier>>(settingsDto.IncomeTiers);
                var socioEconomicPrices = _mapper.Map<List<SocioEconomicPricing>>(settingsDto.SocioEconomicPrices);

                var tiersResult = await _repository.UpdateIncomeDiscountTiersAsync(incomeTiers);
                var pricesResult = await _repository.UpdateSocioEconomicPricingsAsync(socioEconomicPrices);

                if (tiersResult && pricesResult)
                {
                    await _repository.UpdateSystemSettingsAsync(new SystemSettings
                    {
                        SettingName = "DiscountSettings",
                        LastUpdated = DateTime.Now,
                        UpdatedBy = username
                    });
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<double> CalculateDiscountPercentageAsync(double averageMonthlyIncome)
        {
            var tiers = await _repository.GetIncomeDiscountTiersAsync();

            // מיון המדרגות לפי הכנסה מקסימלית בסדר עולה
            var sortedTiers = tiers.OrderBy(t => t.MaxIncome).ToList();

            // חיפוש המדרגה המתאימה
            foreach (var tier in sortedTiers)
            {
                if (averageMonthlyIncome < tier.MaxIncome)
                {
                    return tier.DiscountPercentage;
                }
            }

            // אם לא נמצאה מדרגה מתאימה, אין הנחה
            return 0;
        }

        public async Task<double> GetPricePerSquareMeterAsync(int socioEconomicLevel)
        {
            var pricings = await _repository.GetSocioEconomicPricingsAsync();
            var pricing = pricings.FirstOrDefault(p => p.SocioEconomicLevel == socioEconomicLevel);

            // אם לא נמצאה רמה מתאימה, החזר מחיר ברירת מחדל
            return pricing?.PricePerSquareMeter ?? 20.0;
        }
    }
}
