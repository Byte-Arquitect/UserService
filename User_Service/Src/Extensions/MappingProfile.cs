using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using User_Service.Src.Dtos;
using User_Service.Src.DTOs.Auth;
using User_Service.Src.Models;
using User_Service.Src.Protos;

namespace User_Service.Src.Extensions
{
    public class MappingProfile : Profile
    {
         public MappingProfile()
        {
            CreateMap<User, RegisterResponseDto>();
            CreateMap<RegisterStudentDto, User>();
            CreateMap<RegisterResponseDto, UserRegisterResponse>();
        }
    }
}