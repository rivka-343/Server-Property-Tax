using Microsoft.AspNetCore.Mvc;
using PropertyTax.Core.Services;
using AutoMapper;
using PropertyTax.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Amazon.Auth.AccessControlPolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace PropertyTax.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public UsersController(IUsersService usersService, IMapper mapper)
        {
            _usersService = usersService;
            _mapper = mapper;
        }
       
        [HttpGet]
        [Authorize(Policy = "EmployeeOrManager")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _usersService.GetUsers();
            return Ok(users);
        }

        [HttpGet("Residents")]
        [Authorize(Policy = "EmployeeOrManager")]
        public async Task<IActionResult> GetResidents() {
            var users = await _usersService.GetResidents();
            return Ok(users);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        [Authorize(Policy = "EmployeeOrManager")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _usersService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        
        [HttpPost]
        [Authorize(Policy = "ResidentOnly")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User object is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ודא שאתה מקבל את הסיסמה מה-user object
            var createdUser = await _usersService.Register(user, user.PasswordHash);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

      
        [HttpPut("{id}")]
        [Authorize(Policy = "ResidentOnly")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (updatedUser == null || updatedUser.Id != id)
            {
                return BadRequest("Invalid user data");
            }

            var user = await _usersService.UpdateUser(updatedUser);
            if (user == null)
            {
                return NotFound();
            }

            return NoContent();
        }
       
        
        [HttpDelete("{id}")]
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _usersService.DeleteUser(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
