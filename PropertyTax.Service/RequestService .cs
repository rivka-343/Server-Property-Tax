using Microsoft.AspNetCore.Http;
using PropertyTax.Core.DTO;
using PropertyTax.Core.Models;
using PropertyTax.Core.Repositories;
using PropertyTax.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PropertyTax.DTO;

namespace PropertyTax.Servise
{

    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;

        public RequestService(IRequestRepository requestRepository, IDocumentRepository documentRepository,
            IS3Service s3Service, IMapper mapper)
        {
            _requestRepository = requestRepository;
            _documentRepository = documentRepository;
            _s3Service = s3Service;
            _mapper = mapper;
        }

        //public async Task<int> CreateRequestAsync(Request request)
        //{
        //  var createdRequest = await _requestRepository.CreateRequestAsync(request);
        //    //var documents = new List<Doc>();

        //    //foreach (var file in files)
        //    //{
        //    //  //  var s3Url = await _s3Service.UploadFileAsync(file); // העלאת קובץ ל-S3
        //    //    documents.Add(new Doc { FileName="aaa", ContentType = "zz", RequestId = 1,Request= createdRequest, S3Url = "s3Url" });
        //    //}
        //   // var createdRequest = await _requestRepository.CreateRequestAsync(request);
        ////    await _documentRepository.CreateDocumentsAsync(createdRequest.Documents);

        //    return createdRequest.Id;
        //}
        public async Task<Request> GetRequestByIdAsync(int id)
        {
            return await _requestRepository.GetRequestByIdAsync(id);
        }
        public async Task<RequestStatusDto> GetRequestStatusAsync(int applicationId)
        {
            Request application = await _requestRepository.GetRequestByIdAsync(applicationId);
            if (application == null)
            {
                return null;
            }
            return _mapper.Map<RequestStatusDto>(application);

        }
        public async Task UpdateRequestStatusAsync(int requestId, RequestStatusDto updateRequestStatusDto)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
            {
                return;
            }

            _mapper.Map(updateRequestStatusDto, request);

            await _requestRepository.UpdateRequestAsync(request);
        }
        public async Task UpdateArnonaCalculationAsync(int requestId, CalculateArnonaDto updateArnonaCalculationDto)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
            {
                // טיפול במקרה של בקשה לא קיימת
                return;
            }

            // עדכון חישוב הארנונה באמצעות AutoMapper
            _mapper.Map(updateArnonaCalculationDto, request);

            await _requestRepository.UpdateRequestAsync(request);
        }


        public async Task<int> CreateRequestAsync(Request request)
        {
            var createdRequest = await _requestRepository.CreateRequestAsync(request);
            return createdRequest.Id;
        }
        public async Task AddDocumentsToRequestAsync(int requestId, List<Doc> docs)
        {
            foreach (var doc in docs)
            {
                doc.RequestId = requestId;
                await _documentRepository.CreateDocumentsAsync(doc);
            }
        }

        public async Task<int> CreateRequestWithDocumentsAsync(RequestCreateDto requestCreateDto, int userId)
        {
            var request = _mapper.Map<Request>(requestCreateDto);
            request.Status = "הבקשה נקלטה במערכת";
            request.UserId = userId;
            request.RequestDate = DateTime.Now;

            var createdRequest = await _requestRepository.CreateRequestAsync(request);

            if (requestCreateDto.DocumentUploads != null && requestCreateDto.DocumentUploads.Any())
            {
                var docs = new List<Doc>();
                foreach (var uploadDto in requestCreateDto.DocumentUploads)
                {
                    var doc = new Doc
                    {
                        FileName = uploadDto.FileName,
                        ContentType = uploadDto.ContentType,
                        RequestId = createdRequest.Id,
                        S3Url = uploadDto.S3Url
                    };
                    docs.Add(doc);
                }
                await AddDocumentsToRequestAsync(createdRequest.Id, docs);
            }

            return createdRequest.Id;
        }

        public async Task<Request?> GetUserLatestRequestAsync(string userId)
        {
            return await _requestRepository.GetLatestRequestByUserIdAsync(userId);
        }
    }
}
