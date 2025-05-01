using Microsoft.EntityFrameworkCore;
using PropertyTax.Core.DTO;
using PropertyTax.Core.Models;
using PropertyTax.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Data.Repositories
{
    public class UsersRepository: IUsersRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UsersRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<IEnumerable<ResidentDto>> GetResidents()
        {
            return await _dbContext.Users
                   .Where(u => u.Role == "Resident")
                   .Select(u => new ResidentDto
                   {
                       Id = u.Id,
                       Username = u.Username,
                       IdNumber = u.IdNumber
                   })
                   .ToListAsync();
        }
        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<User> AddUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(User updatedUser)
        {
            _dbContext.Users.Update(updatedUser);
            await _dbContext.SaveChangesAsync();
            return updatedUser;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

      
        public async Task<User> GetUserByUsername(string name)
        {
            return  _dbContext.Users.FirstOrDefault(u => u.Username == name);
        }
    }
}
