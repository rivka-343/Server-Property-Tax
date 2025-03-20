using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Services
{
    public interface IAuthService
    {
        public string GenerateJwtToken(string username, string[] roles, int userId);

        public bool VerifyPassword(string hashedPassword, string providedPassword);

    }
}
