using System.Diagnostics;
using Common.Gprc;
using Grpc.Core;

namespace ApiGateway;

/// <summary>
/// Generic http error response used as a return type when errors occur in controllers
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Type of the error
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Message of the error
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// Name of the method that produced the error
    /// </summary>
    public string? MethodName { get; set; }
    /// <summary>
    /// Stack trace of the error
    /// </summary>
    public string StackTrace { get; set; }
    /// <summary>
    /// Status code of the error
    /// </summary>
    public string? StatusCode { get; set; }

    public ErrorResponse(Exception ex)
    {
        RpcError? rpcError = ex is RpcException ? RpcError.ParseRpcErrorMessage(ex.Message) : null;

        Type = ex.GetType().Name;
        MethodName = new StackTrace(ex).GetFrame(0)?.GetMethod()?.Name;
        StackTrace = ex.ToString();
        Message = rpcError != null ? rpcError.Detail : ex.Message;
        StatusCode = rpcError?.StatusCode;
    }


}