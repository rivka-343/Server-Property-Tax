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
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<List<Doc>> GetDocumentsByRequestIdAsync(int requestId)
        {
            return await _documentRepository.GetDocumentsByRequestIdAsync(requestId);
        }


    }
}
