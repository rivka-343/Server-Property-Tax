using PropertyTax.Core.DTO;
using PropertyTax.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetUsers();
        Task<IEnumerable<ResidentDto>> GetResidents();

        Task<User> GetUserById(int id);
        Task<User> GetUserByUsername(string name);
        Task<User> Register(User user, string password);
        Task<User> UpdateUser(User updatedUser);
        Task<bool> DeleteUser(int id);
    }
}
