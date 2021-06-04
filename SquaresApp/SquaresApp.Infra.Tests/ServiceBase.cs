using AutoMapper;
using SquaresApp.Infra.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaresApp.Infra.Tests
{
    public class ServiceBase
    {
        public readonly IMapper _mapper;
        public ServiceBase()
        { 
            var autoMapperProfiles = new AutoMapperProfiles();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(autoMapperProfiles));
            _mapper = new Mapper(configuration);
        }
    }
}
