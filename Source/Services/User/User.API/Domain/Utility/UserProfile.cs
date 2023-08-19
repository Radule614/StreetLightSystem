using AuthProto;
using AutoMapper;
using Common.Auth;
using Common.Saga.User.Dto;
using User.API.Domain.Entities;
using UserProto;

namespace User.API.Domain.Utility;

/// <summary>
/// Default mapping profile used to configure AutoMapper
/// </summary>
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<UserEntity, UserAuthDto>();
        CreateMap<Role, RoleDto>();
        CreateMap<UserClaims, ClaimData>();
        CreateMap<UserEntity, UserData>();
        CreateMap<UserData, UserEntity>();
        CreateMap<Role, RoleData>();
        CreateMap<RoleData, Role>();
    }
}
