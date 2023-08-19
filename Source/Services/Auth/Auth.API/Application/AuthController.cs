using Auth.API.Services;
using AuthProto;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Auth.API.Application;

/// <summary>
/// AuthController class used for specifying gRPC endpoints for auth micro service
/// </summary>
public class AuthController : AuthGrpc.AuthGrpcBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    /// <summary>
    /// Rpc endpoint for obtaining authentication token.
    /// </summary>
    /// <param name="request">Request that contains email and password.</param>
    /// <param name="context"></param>
    /// <returns>Jwt token.</returns>
    public override async Task<Token> Login(LoginDto request, ServerCallContext context)
    {
        var token = await _authService.Login(request.Email, request.Password);
        return new Token {Token_ = token};
    }

    /// <summary>
    /// Rpc endpoint for validating existing token. Token is passed through bearer authorization header.
    /// </summary>
    /// <param name="request">Request is empty.</param>
    /// <param name="context"></param>
    /// <returns>List of users claims.</returns>
    [Authorize]
    public override Task<UserClaims> ValidateSession(RequiredPermissions request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        IEnumerable<string> permissions = _authService.Authorize(user, request.Permissions.AsEnumerable().ToList());
        var userClaims = new UserClaims
        {
            Id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            Email = user.FindFirstValue(ClaimTypes.Email) ?? "",
            Permissions = { permissions }
        };
        return Task.FromResult(userClaims);
    }
}
