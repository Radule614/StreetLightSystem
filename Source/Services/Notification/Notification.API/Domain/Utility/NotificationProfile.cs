using AuthProto;
using Common.Auth;
using AutoMapper;
using Notification.API.Domain.Entities;
using NotificationProto;
using UserProto;

namespace Notification.API.Domain.Utility;

/// <summary>
/// Default mapping profile used to configure AutoMapper.
/// </summary>
public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<UserClaims, ClaimData>();
        CreateMap<UserDto, Sender>();
        CreateMap<NotificationEntity, MessageDto>();
    }
}