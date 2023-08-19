using Grpc.Core;

namespace Team.API.Domain.Exceptions;

public class TeamInvalidMemberException : RpcException
{
    public TeamInvalidMemberException(Guid teamId, Guid memberId) :
        base(new Status(
            StatusCode.NotFound,
            $"Team has no member with the given memberId. Team Id: {teamId}, Member Id: {memberId}."))
    { }
}
