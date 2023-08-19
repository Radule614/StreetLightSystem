using System.Reflection;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Common.Auth;

/// <summary>
/// Grpc interceptor used to handle authorization.
/// </summary>
/// <typeparam name="TController">Controller type in which the interceptor will be used. Needed for reflection.</typeparam>
public class AuthInterceptor<TController> : Interceptor
{
    private readonly IAuthClient _authClient;
    
    public AuthInterceptor(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    /// <inheritdoc />
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var method = context.Method;
        var methodInfo = GetMethodInfoFromName(method);
        
        if (methodInfo != null)
        {
            var attributes = methodInfo.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                var authAttribute = attribute as AuthAttribute;
                if (authAttribute == null) continue;
                ClaimData claimData = await _authClient.ValidateSession(authAttribute.Permissions, context);
                context.UserState.Add("UserId", claimData.Id);
                context.UserState.Add("Email", claimData.Email);
                context.UserState.Add("Permissions", string.Join(", ", claimData.Permissions));
            }
        }

        var response = await continuation(request, context);
        return response;
    }

    /// <summary>
    /// Method for obtaining method info by reflection. This is used for getting custom attributes (auth attribute specifically).
    /// </summary>
    /// <param name="fullMethodName">Full method path name (namespaces included).</param>
    /// <returns></returns>
    private MethodInfo? GetMethodInfoFromName(string fullMethodName)
    {
        var serviceType = typeof(TController);
        var methodName = fullMethodName.Substring(fullMethodName.LastIndexOf('/') + 1);
        var method = serviceType.GetMethods().FirstOrDefault(m => m.Name.Equals(methodName));
        return method;
    }
}
