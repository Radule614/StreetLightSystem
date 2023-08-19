using AuthProto;
using AutoMapper;
using Common.Auth;
using Common.Saga.User.Dto;
using Team.API.Domain.Entities;
using TeamProto;

namespace Team.API.Domain.Utility;

/// <summary>
/// Default mapping profile used to configure AutoMapper.
/// </summary>
public class TeamProfile : Profile
{
    public TeamProfile()
    {
        CreateMap<UserClaims, ClaimData>();
        CreateMap<Member, UserData>();
        CreateMap<UserData, Member>();
        CreateMap<TeamEntity, TeamDto>();
        CreateMap<TeamEntity, TeamDetailsDto>();
        CreateMap<Member, MemberDto>();
        CreateMap<Member, MemberDetailsDto>();
    }
}
