using PropertyTax.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PropertyTax.Core.Repositories
{
    public interface IRequestRepository
    {
        Task<Request> CreateRequestAsync(Request request);
        Task<Request?> GetLatestRequestByUserIdAsync(string userId);
        Task<Request> GetRequestByIdAsync(int id);
        Task UpdateRequestAsync(Request request);


    }
}
