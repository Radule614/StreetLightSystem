using AuthProto;
using AutoMapper;
using Common.Auth;
using Pole.API.Domain.Entities;
using PoleProto;

namespace Pole.API.Domain.Utility;

/// <summary>
/// Default mapping profile used to configure AutoMapper
/// </summary>
public class PoleProfile : Profile
{
    public PoleProfile()
    {
        CreateMap<PoleEntity, PoleDTO>();
        CreateMap<UserClaims, ClaimData>();
    }
}
