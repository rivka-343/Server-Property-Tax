
using Amazon.Auth.AccessControlPolicy;
using Microsoft.AspNetCore.Mvc;
using PropertyTax.Core.DTO;
using PropertyTax.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace PropertyTax.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]  // רק מנהלים יכולים לגשת לניהול ההגדרות
    [Authorize(Policy = "ManagerOnly")]

    public class DiscountSettingsController : ControllerBase {
        private readonly IDiscountSettingsService _service;

        public DiscountSettingsController(IDiscountSettingsService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<SystemSettingsDto>> GetSettings() {
            var settings = await _service.GetAllSettingsAsync();
            return Ok(settings);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateSettings(SystemSettingsDto settingsDto) {
            var username = User.Identity.Name;
            var result = await _service.UpdateSettingsAsync(settingsDto, username);

            if (result) {
                return Ok(new { message = "ההגדרות עודכנו בהצלחה" });
            }

            return BadRequest(new { message = "אירעה שגיאה בעדכון ההגדרות" });
        }
    }
}
