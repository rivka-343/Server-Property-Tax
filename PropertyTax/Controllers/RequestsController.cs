using Microsoft.AspNetCore.Mvc;
using PropertyTax.Core.Services;
using PropertyTax.DTO;
using PropertyTax.Servise;
using PropertyTax.Core.Models;
using AutoMapper;
using PropertyTax.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;
using PropertyTax.Core.Repositories;
using System.Linq.Expressions;

namespace PropertyTax.Controllers {
    [ApiController]
    [Route("api/[controller]")]

    public class RequestsController : ControllerBase {

        private readonly IRequestService _requestService;
        private readonly IMapper _mapper;
        private readonly IOpenAiService _openAiService;

        public RequestsController(IRequestService requestService, IMapper mapper, IOpenAiService openAiService) {
            _requestService = requestService;
            _mapper = mapper;
            _openAiService = openAiService;
        }

        [HttpPost("Chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request) {
            //string response = await _openAiService.GetChatResponse(request.Message);
            //return Ok(new { Response = response });
            var reply = await _openAiService.GetChatResponse(request.Message, "Please extract the values from the \"Credit\" column and return only their total sum as a number, without any additional text or formatting.\nIf you're unsure which column is \"Credit\", return the sum of the rightmost numeric column.\nUse standard English number format");
            return Ok(new { response = reply });
        }
        [HttpGet("xx")]
        public async Task<IEnumerable<string>> ListModelsAsync() {

            return await _openAiService.ListModelsAsync();
        }


        [HttpGet("run")]
        public async Task<string> run() {

            return await _openAiService.Run();
        }

        //[HttpPost("CreateRequest")]
        //[Authorize(Policy = "ResidentOnly")]
        //public async Task<IActionResult> CreateRequest([FromBody] RequestCreateDto requestCreateDto) {
        //    int userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int parsedUserId) ? parsedUserId : 0;
        //    var requestId = await _requestService.CreateRequestWithDocumentsAsync(requestCreateDto, userId);
        //    return CreatedAtAction(nameof(GetRequest), new { id = requestId }, new { id = requestId });
        //}

        [HttpPost("CreateRequest")]
        [Authorize(Policy = "ResidentOnly")]
        public async Task<IActionResult> CreateRequest([FromBody] RequestCreateDto requestCreateDto) {
            int userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int parsedUserId) ? parsedUserId : 0;

            // רשימת סוגי המסמכים הנדרשים
            var requiredTypes = new List<DocumentType>
            {
                DocumentType.IDWithSpousePage,
                DocumentType.BankStatement,
                DocumentType.PayslipSpouse1,
                DocumentType.PayslipSpouse2
            };
            try {
                if (requestCreateDto.DocumentUploads == null || requestCreateDto.DocumentUploads.Count != 4) {
                    return BadRequest("חובה לצרף 4 מסמכים.");
                }
            
            var uploadedTypes = requestCreateDto.DocumentUploads.Select(d => d.Type).ToList();

            if (!requiredTypes.All(t => uploadedTypes.Contains(t))) {
                return BadRequest("חובה לצרף את כל סוגי המסמכים הנדרשים.");
            }

            var requestId = await _requestService.CreateRequestWithDocumentsAsync(requestCreateDto, userId);
            return CreatedAtAction(nameof(GetRequest), new { id = requestId }, new { id = requestId });
            }

            catch (ArgumentException ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AuthenticatedUsers")]
        public async Task<IActionResult> GetRequest(int id) {
            var request = await _requestService.GetRequestByIdAsync(id);

            if (request == null) {
                return NotFound();
            }

            return Ok(request);
        }

        [HttpGet]
        [Authorize(Policy = "EmployeeOrManager")]
        public async Task<IActionResult> GetRequest() {
            var request = await _requestService.GetRequestsAsync();

            if (request == null) {
                return NotFound();
            }

            return Ok(request);
        }

        [HttpGet("my-request")]
        [Authorize(Policy = "ResidentOnly")]
        public async Task<IActionResult> GetUserRequest() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var request = await _requestService.GetUserLatestRequestAsync(userId);
            return request != null ? Ok(request) : NotFound();
        }

        [HttpGet("{id}/status")]
        [Authorize(Policy = "AuthenticatedUsers")]
        public async Task<IActionResult> GetApplicationStatus(int id) {
            var status = await _requestService.GetRequestStatusAsync(id);
            if (status == null) {
                return NotFound();
            }

            return Ok(status);
        }

        [HttpPut("{id}/status")]
        [Authorize(Policy = "EmployeeOrManager")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] RequestStatusDto updateRequestStatusDto) {

            await _requestService.UpdateRequestStatusAsync(id, updateRequestStatusDto);
            return NoContent();
        }

        [HttpPut("{id}/calculation")]
        [Authorize(Policy = "EmployeeOrManager")]
        public async Task<IActionResult> UpdateArnonaCalculation(int id, [FromBody] CalculateArnonaDto updateArnonaCalculationDto) {
            await _requestService.UpdateArnonaCalculationAsync(id, updateArnonaCalculationDto);
            return NoContent();
        }

    }
}
