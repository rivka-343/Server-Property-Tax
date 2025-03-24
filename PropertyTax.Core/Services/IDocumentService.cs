using PropertyTax.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Services
{
    public interface IDocumentService
    {
        Task<List<Doc>> GetDocumentsByRequestIdAsync(int requestId);
    }
}
