using Grpc.Core;

namespace Team.API.Domain.Exceptions;

public class MemberHasNoTeamException : RpcException
{
    public MemberHasNoTeamException(Guid memberId) :
        base(new Status(
            StatusCode.NotFound,
            $"Member has no team: {memberId}"))
    { }
}
