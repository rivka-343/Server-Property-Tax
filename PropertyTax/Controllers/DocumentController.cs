using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyTax.Core.Models;
using PropertyTax.Core.Services;
using PropertyTax.Servise;

namespace PropertyTax.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : Controller {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService) {
            _documentService= documentService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>  GetDocumentsByRequestIdAsync(int id) {
            var documents = await _documentService.GetDocumentsByRequestIdAsync(id);

            if (documents == null) {
                return NotFound();
            }
            return Ok(documents);
        }
    }
}
