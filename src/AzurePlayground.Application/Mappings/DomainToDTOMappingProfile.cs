using AutoMapper;
using AzurePlayground.Application.DTOs;
using AzurePlayground.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzurePlayground.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile()
        {
            CreateMap<Document, DocumentDTO>().ReverseMap();
        }
    }
}
