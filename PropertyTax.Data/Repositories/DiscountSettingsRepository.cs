using Microsoft.EntityFrameworkCore;
using PropertyTax.Core.Models;
using PropertyTax.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Data.Repositories
{
    public class DiscountSettingsRepository : IDiscountSettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountSettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<IncomeDiscountTier>> GetIncomeDiscountTiersAsync()
        {
            return await _context.IncomeDiscountTiers.OrderBy(t => t.MaxIncome).ToListAsync();
        }

        public async Task<List<SocioEconomicPricing>> GetSocioEconomicPricingsAsync()
        {
            return await _context.SocioEconomicPricings.OrderBy(p => p.SocioEconomicLevel).ToListAsync();
        }

        public async Task<bool> UpdateIncomeDiscountTiersAsync(List<IncomeDiscountTier> tiers)
        {
            try
            {
                // מחיקת כל המדרגות הקיימות
                var existingTiers = await _context.IncomeDiscountTiers.ToListAsync();
                _context.IncomeDiscountTiers.RemoveRange(existingTiers);

                // הוספת המדרגות החדשות
                await _context.IncomeDiscountTiers.AddRangeAsync(tiers);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateSocioEconomicPricingsAsync(List<SocioEconomicPricing> pricings)
        {
            try
            {
                // מחיקת כל המחירים הקיימים
                var existingPricings = await _context.SocioEconomicPricings.ToListAsync();
                _context.SocioEconomicPricings.RemoveRange(existingPricings);

                // הוספת המחירים החדשים
                await _context.SocioEconomicPricings.AddRangeAsync(pricings);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<SystemSettings> GetSystemSettingsAsync(string settingName)
        {
            return await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingName == settingName);
        }

        public async Task UpdateSystemSettingsAsync(SystemSettings settings)
        {
            var existingSettings = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingName == settings.SettingName);

            if (existingSettings == null)
            {
                await _context.SystemSettings.AddAsync(settings);
            }
            else
            {
                existingSettings.LastUpdated = settings.LastUpdated;
                existingSettings.UpdatedBy = settings.UpdatedBy;
                _context.SystemSettings.Update(existingSettings);
            }

            await _context.SaveChangesAsync();
        }
    }
}
