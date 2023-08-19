using Grpc.Core;

namespace ApiGateway.Middleware;

/// <summary>
/// Middleware for extracting http headers to metadata object which is required by grpc calls.
/// </summary>
public class MetadataMiddleware
{
    private readonly RequestDelegate _next;

    public MetadataMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var metadata = new Metadata();
        foreach (var header in context.Request.Headers)
        {
            metadata.Add(header.Key, header.Value!);
        }
        context.Items["Metadata"] = metadata;
        await _next(context);
    }
}
