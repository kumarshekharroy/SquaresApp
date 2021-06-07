using AutoMapper;
using SquaresApp.Application.Profiles;

namespace SquaresApp.Application.Tests
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
