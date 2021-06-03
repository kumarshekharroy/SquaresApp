using AutoMapper;
using SquaresApp.Common.DTOs;
using SquaresApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaresApp.Infra.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, GetUserDTO>().ForMember(dto => dto.Password, opt => opt.UseDestinationValue());
            CreateMap<UserDTO, User>(); 
        }
    }
}
