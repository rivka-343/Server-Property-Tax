using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PropertyTax.Core.Services;
using Microsoft.AspNetCore.Identity;
using PropertyTax.Core.Models;

namespace PropertyTax.Servise
{
    public class AuthService: IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;

        
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;

        }
        public string GenerateJwtToken(string username, string[] roles, int userId)
        {
            Console.WriteLine(username);
            Console.WriteLine(roles[0]);
            Console.WriteLine(userId);
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new Exception("JWT_KEY is missing in environment variables.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            Console.WriteLine(securityKey);
            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>{
            new Claim(ClaimTypes.Name, username),
            new Claim("id", userId.ToString()) // הוספת ה-ID של המשתמש כ-Claim
            };

            // הוספת תפקידים כ-Claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(null, hashedPassword, providedPassword) == PasswordVerificationResult.Success;
        }
    }
}
