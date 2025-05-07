using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyTax.Core.Models;
using PropertyTax.Core.Services;
using PropertyTax.Servise;
using Microsoft.AspNetCore.Authorization;

namespace PropertyTax.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : Controller {
        private readonly IDocumentService _documentService;
        private readonly IS3Service _s3Service;
        public DocumentController(IDocumentService documentService, IS3Service s3Service) {
            _documentService = documentService;
            _s3Service = s3Service;
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult>  GetDocumentsByRequestIdAsync(int id) {
        //    var documents = await _documentService.GetDocumentsByRequestIdAsync(id);

        //    if (documents == null) {
        //        return NotFound();
        //    }
        //    return Ok(documents);
        //}
        [HttpGet("request-files/{requestId}")]
        [Authorize(Policy = "AuthenticatedUsers")]
        public async Task<IActionResult> GetFilesByRequestId(int requestId) {
            // שליפת כל הקבצים לפי מזהה הבקשה

            var files = await _documentService.GetDocumentsByRequestIdAsync(requestId);

            if (files == null || files.Count == 0) {
                return NotFound("No files found for the given request ID.");
            }

            var fileUrls = new List<string>();

            foreach (var file in files) {
                // file.FileKey מכיל את הנתיב של הקובץ ב-S3
                var downloadUrl = await _s3Service.GetDownloadUrlAsync(file.S3Url);
                fileUrls.Add(downloadUrl);
            }

            return Ok(new { files = fileUrls });
        }

    }
}
