using Microsoft.EntityFrameworkCore;
using PropertyTax.Core.Models;
using PropertyTax.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Data.Repositories
{
    public class RequestRepository:IRequestRepository
    {
        private readonly ApplicationDbContext _dbContext; // הקשר למסד הנתונים

        public RequestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Request> CreateRequestAsync(Request request)
        {
            _dbContext.Requests.Add(request);
            await _dbContext.SaveChangesAsync();
            return request;
        }

        public async Task<Request?> GetLatestRequestByUserIdAsync(string userId)
        {
            return await _dbContext.Requests
        .Where(r => r.UserId.ToString().Equals(userId))
       // .OrderByDescending(r => r.CreatedAt)
        .FirstOrDefaultAsync();
        }

        public async Task<Request> GetRequestByIdAsync(int id)
        {
            return await _dbContext.Requests.FindAsync(id);
        }

        public async Task UpdateRequestAsync(Request request)
        {
            _dbContext.Requests.Update(request);
            await _dbContext.SaveChangesAsync();
        }
    }
}