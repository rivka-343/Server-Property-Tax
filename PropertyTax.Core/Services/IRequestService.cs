using Microsoft.AspNetCore.Http;
using PropertyTax.Core.DTO;
using PropertyTax.Core.Models;
using PropertyTax.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyTax.Core.Services
{
    public interface IRequestService
    {
        Task<int> CreateRequestAsync(Request request);
        Task<Request> GetRequestByIdAsync(int id);
        Task<RequestStatusDto> GetRequestStatusAsync(int requestId);
        Task UpdateRequestStatusAsync(int requestId, RequestStatusDto updateRequestStatusDto);
        Task UpdateArnonaCalculationAsync(int requestId, CalculateArnonaDto updateArnonaCalculationDto);
        Task AddDocumentsToRequestAsync(int requestId, List<Doc> docs);
        Task<int> CreateRequestWithDocumentsAsync(RequestCreateDto requestCreateDto, int userId);
        Task<Request?> GetUserLatestRequestAsync(string userId);
    }
}
