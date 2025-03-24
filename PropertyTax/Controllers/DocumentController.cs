using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyTax.Core.Services;

namespace PropertyTax.Controllers {
    public class DocumentController : Controller {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService) {
            _documentService= documentService;
        }

    }
}
