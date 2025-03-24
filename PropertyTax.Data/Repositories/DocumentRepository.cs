using Microsoft.EntityFrameworkCore;
using PropertyTax.Core.Models;
using PropertyTax.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DocumentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Doc> CreateDocumentsAsync(Doc doc)
        {
            _dbContext.Documents.Add(doc);
            await _dbContext.SaveChangesAsync();
            return doc;
        }

        public async Task<List<Doc>> GetDocumentsByRequestIdAsync(int requestId)
        {
              return await _dbContext.Documents.Where(doc => doc.RequestId == requestId).ToListAsync();
        }
    }
}
