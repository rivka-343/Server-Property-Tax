using PropertyTax.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Repositories
{
    public interface IPropertyRepository
    {
        Task<PropertyBaseData> GetByPropertyNumberAsync(int propertyNumber);

    }
}
