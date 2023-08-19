using AuthProto;
using Common;
using Common.Gprc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase, IDisposable
{
    /// <summary>
    /// Auth service channel used by auth grpc client.
    /// </summary>
    private readonly GrpcChannel _authServiceChannel;
    /// <summary>
    /// Auth gprc client used to make grpc requests.
    /// </summary>
    private readonly AuthGrpc.AuthGrpcClient _authClient;
    public AuthController(IChannelFactory factory, IConfiguration configuration)
    {
        _authServiceChannel = factory.GetChannel(configuration[Constants.AuthServiceAddress]!);
        _authClient = new AuthGrpc.AuthGrpcClient(_authServiceChannel);
    }

    public void Dispose()
    {
        _authServiceChannel.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Route for login.
    /// </summary>
    /// <param name="data">Email and password dto.</param>
    /// <returns>Jwt token.</returns>
    [HttpPost("login")]
    public async Task<Token> Login(LoginDto data)
    {
        Metadata? metadata = Request.HttpContext.Items["Metadata"] as Metadata;
        return await _authClient.LoginAsync(data, metadata);
    }
}
