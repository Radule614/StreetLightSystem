using AuthProto;
using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;

namespace Notification.API.Infrastructure;

/// <inheritdoc />
public class AuthClient : IAuthClient
{
    /// <summary>
    /// Auth service channel used by auth grpc client
    /// </summary>
    private readonly GrpcChannel _authServiceChannel;
    /// <summary>
    /// Auth gprc client used to make grpc requests.
    /// </summary>
    private readonly AuthGrpc.AuthGrpcClient _authClient;
    private readonly IMapper _mapper;
    public AuthClient(IChannelFactory channelFactory, IConfiguration configuration, IMapper mapper)
    {
        _authServiceChannel = channelFactory.GetChannel(configuration[Constants.AuthServiceAddress]!);
        _authClient = new AuthGrpc.AuthGrpcClient(_authServiceChannel);
        _mapper = mapper;
    }

    ~AuthClient()
    {
        Dispose(false);
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _authServiceChannel.Dispose();
    }

    /// <inheritdoc/>
    public async Task<ClaimData> ValidateSession(IEnumerable<string> permissions, ServerCallContext context)
    {
        var requiredPermissions = new RequiredPermissions
        {
            Permissions = { permissions }
        };
        UserClaims userClaims = await _authClient.ValidateSessionAsync(requiredPermissions, context.RequestHeaders);
        return _mapper.Map<ClaimData>(userClaims);
    }
}
