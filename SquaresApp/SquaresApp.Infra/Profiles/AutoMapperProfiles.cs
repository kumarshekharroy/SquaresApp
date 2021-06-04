using AutoMapper;
using SquaresApp.Common.Constants;
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
            CreateMap<User, GetUserDTO>().ForMember(dto => dto.Password, opt => opt.Ignore());
            CreateMap<UserDTO, User>();

            CreateMap<Point, GetPointDTO>();
            CreateMap<PointDTO, Point>().ForMember(d => d.UserId,
                opt => opt.MapFrom(
                    (src, dst, _, context) => context.Options.Items[ConstantValues.UserId]
                )
            );
        }
    }
}
