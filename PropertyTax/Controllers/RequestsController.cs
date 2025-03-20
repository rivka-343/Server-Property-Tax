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

namespace PropertyTax.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IMapper _mapper;
        private readonly IOpenAiService _openAiService;

        public RequestsController(IRequestService requestService, IMapper mapper, IOpenAiService openAiService)
        {
            _requestService = requestService;
            _mapper = mapper;
            _openAiService = openAiService;
        }

        [HttpPost("Chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request) {
            string response = await _openAiService.GetChatResponse(request.Message);
            return Ok(new { Response = response });
        }

        //[HttpPost("CreateRequest")]
        //public async Task<IActionResult> CreateRequest([FromForm] RequestCreateDto requestCreateDto)
        //{
        //    var request = _mapper.Map<Request>(requestCreateDto);
        //    request.Status = "הבקשה נקלטה במערכת"; // אתחול הסטטוס לערך הרצוי
        //    request.UserId = int.TryParse(User.FindFirst("id")?.Value, out int userId) ? userId : 0;
        //    request.RequestDate = DateTime.Now;
        //    var requestId = await _requestService.CreateRequestAsync(request);
        //    return CreatedAtAction(nameof(GetRequest), new { id = requestId }, new { id = requestId });
        //}

        [HttpPost("CreateRequest")]
        [Authorize(Policy = "ResidentOnly")]
        public async Task<IActionResult> CreateRequest([FromBody] RequestCreateDto requestCreateDto) {
            int userId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int parsedUserId) ? parsedUserId : 0;
            var requestId = await _requestService.CreateRequestWithDocumentsAsync(requestCreateDto, userId);
            return CreatedAtAction(nameof(GetRequest), new { id = requestId }, new { id = requestId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequest(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id); // נניח שיש לך פונקציה כזו ב-IRequestService

            if (request == null)
            {
                return NotFound(); // החזר 404 אם הבקשה לא נמצאה
            }

            return Ok(request); // החזר 200 עם הבקשה
        }
    
        [HttpGet("{id}/status")]
        public async Task<IActionResult> GetApplicationStatus(int id)
        {
            var status = await _requestService.GetRequestStatusAsync(id);
            if (status == null)
            {
                return NotFound();
            }

            return Ok(status);
        }
        
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] RequestStatusDto updateRequestStatusDto)
        {
            await _requestService.UpdateRequestStatusAsync(id, updateRequestStatusDto);
            return NoContent();
        }
       
        [HttpPut("{id}/calculation")]
        public async Task<IActionResult> UpdateArnonaCalculation(int id, [FromBody] CalculateArnonaDto updateArnonaCalculationDto)
        {
            await _requestService.UpdateArnonaCalculationAsync(id, updateArnonaCalculationDto);
            return NoContent();
        }

    }
}
