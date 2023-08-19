using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common;
using Common.Gprc;
using Common.Gprc.Exceptions;
using Grpc.Net.Client;
using Microsoft.IdentityModel.Tokens;
using UserProto;

namespace Auth.API.Services;

/// <summary>
/// Auth Service used to manage complex domain logic.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly IConfiguration _configuration;
    /// <summary>
    /// User service channel used by user grpc client
    /// </summary>
    private readonly GrpcChannel _userServiceChannel;
    /// <summary>
    /// User gprc client used to make grpc requests.
    /// </summary>
    private readonly UserGrpc.UserGrpcClient _userClient;
    public AuthService(IConfiguration configuration, IChannelFactory factory, ILogger<AuthService> logger)
    {
        _configuration = configuration;
        _userServiceChannel = factory.GetChannel(configuration[Constants.UserServiceAddress]!);
        _userClient = new UserGrpc.UserGrpcClient(_userServiceChannel);
        _logger = logger;
    }

    public void Dispose()
    {
        _userServiceChannel.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<string> Login(string email, string password)
    {
        UserAuthDto user = await _userClient.GetByAuthEmailAsync(new Email { Email_ = email });
        if (!user.Password.Equals(PasswordValidator.HashPassword(password)))
        {
            throw new AuthenticationException("Password is not valid.");
        }
        return CreateToken(user.Id, email, user.Permissions);
    }

    public IEnumerable<string> Authorize(ClaimsPrincipal user, List<string> requiredPermissions)
    {
        _logger.LogInformation($"Trying to authorize user: ${user.FindFirstValue(ClaimTypes.Email)}");
        IEnumerable<string>? permissions = user.FindFirstValue(ClaimTypes.Role)?.Split(";");
        if (permissions == null)
        {
            throw new AuthorizationException(new List<string>(), requiredPermissions);
        }

        if (requiredPermissions.IsNullOrEmpty() || requiredPermissions.Any(permission => permissions.Contains(permission)))
        {
            _logger.LogInformation($"User authorized: ${user.FindFirstValue(ClaimTypes.Email)}");
            return permissions;
        }
        throw new AuthorizationException(permissions, requiredPermissions);
    }

    /// <summary>
    /// Method for creating new jwt tokens.
    /// </summary>
    /// <param name="userId">Id of the user, will be placed into the jwt claims.</param>
    /// <param name="email">Email of the user, will be placed into the jwt claims.</param>
    /// <param name="requiredPermissions">List of permissions. User must have at least one.</param>
    /// <returns></returns>
    private string CreateToken(string userId, string email, IEnumerable<string> requiredPermissions)
    {

        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!);
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, string.Join(";", requiredPermissions))
        };
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(4),
            Issuer = _configuration["JwtSettings:Issuer"]!,
            Audience = _configuration["JwtSettings:Audience"]!,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
