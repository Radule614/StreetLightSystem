using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiGateway.Controllers;

/// <summary>
/// Error controller is responsible for handling errors occurred in other, regular controllers.
/// </summary>
[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    /// <summary>
    /// Extracts exception data and creates a consistent error response.
    /// </summary>
    /// <returns>Consistent error response that contains status code, message and stack trace</returns>
    [Route("Error")]
    public ErrorResponse Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;
        Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        ErrorResponse errorResponse = new(exception ?? new Exception("Unknown error has occurred."));
        if (errorResponse.StatusCode == null) return errorResponse;
        
        Response.StatusCode = errorResponse.StatusCode switch
        {
            "NotFound" => (int)HttpStatusCode.NotFound,
            "PermissionDenied" => (int)HttpStatusCode.Forbidden,
            "Unauthenticated" => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.BadRequest
        };
        return errorResponse;
    }
}