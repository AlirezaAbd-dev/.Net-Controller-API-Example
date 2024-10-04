using AutoMapper;
using Cityinfo.API.Entities;
using Cityinfo.API.Models;
using Cityinfo.API.utilities;

namespace Cityinfo.API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterRequest, User>()
            .ForMember(p => p.Name, opt =>
                opt.MapFrom(p => p.UserName))
            .ForMember(p => p.Password, opt =>
                opt.MapFrom(p => p.Password.ToHashString()));

        CreateMap<User, UserResponse>();
    }
}
