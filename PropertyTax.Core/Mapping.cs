using Amazon;
using PropertyTax.Core.Models;
using PropertyTax.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using static System.Net.Mime.MediaTypeNames;
using PropertyTax.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace PropertyTax.Core
{
    public class Mapping : AutoMapper.Profile
    {
        private readonly IMapper _mapper;
        public Mapping()
        {

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<RequestCreateDto, Request>().ReverseMap();

            //  CreateMap<Request, RequestCreateDto>().ReverseMap()
            //.ForMember(dest => dest.Documents, opt => opt.MapFrom(src =>
            //    src.Documents.Select(file => new Doc
            //    {
            //        FileName = file.FileName,
            //        ContentType ="mmmm",
            //        S3Url = "http://your-static-url.com/" // ערך סטטי
            //    }).ToList()));
            CreateMap<Request, RequestStatusDto>().ReverseMap();
            CreateMap<Request, CalculateArnonaDto>().ReverseMap();
            
           // CreateMap<IFormFile, Doc>()
           //.ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
           //.ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
           //.ForMember(dest => dest.S3Url, opt => opt.MapFrom(src => "http://your-static-url.com/")); // ערך סטטי

        }
    }
}
