using System.Security.Claims;

namespace Auth.API.Services;

public interface IAuthService : IDisposable
{
    /// <summary>
    /// Method for obtaining authentication token by email and password.
    /// </summary>
    /// <param name="email">Email of the user.</param>
    /// <param name="password">Password of the user.</param>
    /// <returns>Jwt token.</returns>
    Task<string> Login (string email, string password);

    /// <summary>
    /// Method for authorizing user roles.
    /// </summary>
    /// <param name="user">Roles principal containing all user roles.</param>
    /// <param name="requiredPermissions">List of permissions that the user can have in order to access the resource.</param>
    /// <returns>The list of user roles.</returns>
    IEnumerable<string> Authorize(ClaimsPrincipal user, List<string> requiredPermissions);
}
