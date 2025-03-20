using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyTax.Core.Models;
using PropertyTax.Core.Services;
using PropertyTax.DTO;

namespace PropertyTax.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsersService _usersService;

        public AuthController(IAuthService authService, IUsersService usersService)
        {
            _authService = authService;
            _usersService = usersService;
        }

        //[HttpPost("login")]
        //public IActionResult Login([FromBody] Login model)
        //{
        //    // כאן יש לבדוק את שם המשתמש והסיסמה מול מסד הנתונים
        //    if (model.UserName == "admin" && model.Password == "admin123")
        //    {
        //        var token = _authService.GenerateJwtToken(model.UserName, new[] { "Admin" });
        //        return Ok(new { Token = token });
        //    }
        //    else if (model.UserName == "editor" && model.Password == "editor123")
        //    {
        //        var token = _authService.GenerateJwtToken(model.UserName, new[] { "Editor" });
        //        return Ok(new { Token = token });
        //    }
        //    else if (model.UserName == "viewer" && model.Password == "viewer123")
        //    {
        //        var token = _authService.GenerateJwtToken(model.UserName, new[] { "Viewer" });
        //        return Ok(new { Token = token });
        //    }
        //    return Unauthorized();
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _usersService.GetUserByUsername(model.UserName);
            if (user == null || !_authService.VerifyPassword(user.PasswordHash, model.Password))
            {
                return Unauthorized();
            }
            var token = _authService.GenerateJwtToken(user.Username, new[] { user.Role },user.Id);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Username = model.Username,
                IdNumber = model.IdNumber,
                Role = model.Role
            };

            var createdUser = await _usersService.Register(user, model.Password);
            return CreatedAtRoute("GetUserById", new { id = createdUser.Id }, createdUser);
            //var createdUser = await _usersService.Register(user, model.Password);
            //return CreatedAtAction(nameof(UsersController.GetUser), new { id = createdUser.Id }, createdUser);


        }
    }
}
