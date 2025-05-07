using PropertyTax.Core.Models;
using PropertyTax.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Data.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _dbContext; // הקשר למסד הנתונים

        public PropertyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PropertyBaseData> GetByPropertyNumberAsync(int propertyNumber)
        {
            return await _dbContext.PropertyBaseData.FindAsync(propertyNumber);
        }
    }
}