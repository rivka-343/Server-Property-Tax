using PropertyTax.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Repositories
{
    public interface IDocumentRepository
    {
        Task<Doc> CreateDocumentsAsync(Doc doc);
    }
}
