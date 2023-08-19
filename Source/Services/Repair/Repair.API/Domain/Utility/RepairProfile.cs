using AuthProto;
using AutoMapper;
using Common.Auth;
using Common.Saga.Repair.Dto;
using Repair.API.Domain.Entities;
using RepairProto;

namespace Repair.API.Domain.Utility;

/// <summary>
/// Default mapping profile used to configure AutoMapper
/// </summary>
public class RepairProfile : Profile
{
    public RepairProfile()
    {
        CreateMap<RepairEntity, RepairDto>();
        CreateMap<UserClaims, ClaimData>();
        CreateMap<RepairEntity, RepairData>();
    }
}
