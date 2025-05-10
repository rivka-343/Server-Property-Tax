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
            CreateMap<Request, RequestStatusDto>().ReverseMap();
            CreateMap<Request, CalculateArnonaDto>().ReverseMap();
            CreateMap<IncomeDiscountTierDto, IncomeDiscountTier>().ReverseMap();
            CreateMap<SocioEconomicPricingDto, SocioEconomicPricing>().ReverseMap();
        }
    }
}
