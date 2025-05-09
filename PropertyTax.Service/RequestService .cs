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
using PropertyTax.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;
using UglyToad.PdfPig.Content;

namespace PropertyTax.Servise
{

    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IS3Service _s3Service;
        private readonly IMapper _mapper;
        private readonly IOpenAiService _iopenAiService;
        private readonly IDocumentService _documentService;

        public RequestService(IRequestRepository requestRepository, IDocumentRepository documentRepository, IPropertyRepository propertyRepository,
            IS3Service s3Service, IMapper mapper, IOpenAiService iopenAiService, IDocumentService documentService)
        {
            _requestRepository = requestRepository;
            _documentRepository = documentRepository;
            _propertyRepository = propertyRepository;
            _s3Service = s3Service;
            _mapper = mapper;
            _iopenAiService = iopenAiService;
            _documentService = documentService;
        }

        public async Task<Request> GetRequestByIdAsync(int id)
        {
            return await _requestRepository.GetRequestByIdAsync(id);
        }
        public async Task<IEnumerable<RequestMinimalDto>> GetRequestsAsync()
        {
            var requests = await _requestRepository.GetRequestsAsync();
            return requests.Select(r => new RequestMinimalDto
            {
                Id = r.Id,
                UserId = r.UserId,
                Status = r.Status,
                RequestDate = r.RequestDate
            }).ToList();
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
        public async Task<T> AnalyzeDocumentAsync<T>(string S3Url, string prompt)
        {
            var downloadUrl = await _s3Service.GetDownloadUrlAsync(S3Url);
            var response = await _iopenAiService.GetChatResponse(downloadUrl, prompt);
            try
            {
                return (T)Convert.ChangeType(response, typeof(T));
            }
            catch
            {
                return (T)Convert.ChangeType(0, typeof(T));
            }
        }
        //****
        //בפונקציה הזו מנסים לאמת נתונים בין תלוש המשכורת לבין הנתונים בבנק 
        //האימות מתבצע ע"י הAI
        //ולכן הוא לא מספיק אמין ונכשל הרבה פעמים
        //לכן הוא נמצא בהערה
        //******
        public async Task<int> CreateRequestWithDocumentsAsync(RequestCreateDto requestCreateDto, int userId)
        {
            var request = _mapper.Map<Request>(requestCreateDto);
            request.Status = "הבקשה נקלטה במערכת";
            request.UserId = userId;
            request.RequestDate = DateTime.Now;
            var createdRequest = await _requestRepository.CreateRequestAsync(request);
            if (requestCreateDto.DocumentUploads != null && requestCreateDto.DocumentUploads.Any())
            {
                var docs = requestCreateDto.DocumentUploads.Select(uploadDto => new Doc
                {
                    FileName = uploadDto.FileName,
                    ContentType = uploadDto.ContentType,
                    RequestId = createdRequest.Id,
                    S3Url = uploadDto.S3Url,
                    Type = uploadDto.Type
                }).ToList();

                await AddDocumentsToRequestAsync(createdRequest.Id, docs);

                double bankIncome = await AnalyzeDocumentAsync<double>(docs[1].S3Url, "You are analyzing an Israeli bank statement.\n" + "Please sum all incoming transactions that appear to be salary deposits (typically with descriptions such as \"salary\", \"pay\", \"תלוש\", etc).\n" + "Return only the total sum of these transactions as a number with no text.");

                int numberPeople = await AnalyzeDocumentAsync<int>(docs[0].S3Url, "You will receive an Israeli ID card and an attached \"Sefach\" (family appendix).\nFrom these documents, please identify how many children are listed in the family.\nReturn only the number of children as a digit (e.g., 3), with no extra explanation or text.\nIf the information is unclear or incomplete, return 0.");

                double broto1 = await AnalyzeDocumentAsync<double>(docs[2].S3Url, "You will receive a payslip. From this document, extract the final net amount that was actually transferred to the employee's bank account. Return only the amount, as a number (e.g., 8421.55), with no additional words. Be precise and careful — if you are not 100% certain of the correct amount, do not guess or invent a value. In such case, return null.");

                double broto2 = await AnalyzeDocumentAsync<double>(docs[3].S3Url, "You will receive a payslip. From this document, extract the final net amount that was actually transferred to the employee's bank account. Return only the amount, as a number (e.g., 8421.55), with no additional words. Be precise and careful — if you are not 100% certain of the correct amount, do not guess or invent a value. In such case, return null.");

                double totalPaySlipsIncome = broto1 + broto2;

                Console.WriteLine($"סכום בבנק: {bankIncome}, סכום בתלושים: {totalPaySlipsIncome}");

                //if (Math.Abs(bankIncome - totalPaySlipsIncome) <= 100)
                //{ 
                request.AverageMonthlyIncome = totalPaySlipsIncome / (numberPeople + 2);

                double discountPercentage;

                // מדרגות הנחה
                if (request.AverageMonthlyIncome < 2000)
                    discountPercentage = 90;
                else if (request.AverageMonthlyIncome < 3000)
                    discountPercentage = 70;
                else if (request.AverageMonthlyIncome < 4000)
                    discountPercentage = 50;
                else if (request.AverageMonthlyIncome < 5000)
                    discountPercentage = 30;
                else if (request.AverageMonthlyIncome < 6000)
                    discountPercentage = 10;
                else
                    discountPercentage = 0;

                request.ApprovedArnona = discountPercentage;

                PropertyBaseData p = await _propertyRepository.GetByPropertyNumberAsync(request.PropertyNumber);
                // התייחסות לרמה סוציו-אקונומית
                double priceForMeter = 20;
                int socioeconomicLevel = p.SocioEconomicLevel;
                if (socioeconomicLevel == 4)
                {
                    priceForMeter = 17.5;
                }
                if (socioeconomicLevel == 3)
                {
                    priceForMeter = 15.2;
                }
                if (socioeconomicLevel == 2)
                {
                    priceForMeter = 12.5;
                }
                if (socioeconomicLevel == 1)
                {
                    priceForMeter = 10;
                }
                request.CalculatedArnona = (p.AreaInSquareMeters) * priceForMeter *
                        (1 - (request.ApprovedArnona / 100));
                //}
                //else
                //  {
                //  request.AverageMonthlyIncome = 0;
                //  request.Status="האימות עם הבנק נכשל יש לבדוק שוב ידנית"
                //  }
            }

            await _requestRepository.UpdateRequestAsync(request);
            return createdRequest.Id;
        }
        public async Task<Request?> GetUserLatestRequestAsync(string userId)
        {
            return await _requestRepository.GetLatestRequestByUserIdAsync(userId);
        }
        //public async Task<Request> GetRequestsAsync()
        //{
        //    return await _requestRepository.GetRequestsAsync();
        //}

    }
}
