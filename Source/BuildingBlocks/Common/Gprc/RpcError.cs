using System.Text.RegularExpressions;

namespace Common.Gprc;

/// <summary>
/// Error class for extracting and storing rpc errors thrown by gRPC services.
/// </summary>
public class RpcError
{
    /// <summary>
    /// Rpc status code
    /// </summary>
    public string StatusCode { get; set; }
    /// <summary>
    /// Rpc error message
    /// </summary>
    public string Detail { get; set; }

    private RpcError()
    {
        StatusCode = string.Empty;
        Detail = string.Empty;
    }

    /// <summary>
    /// Creates RpcError object from serialized rpc exception.
    /// </summary>
    /// <param name="rpcException">Serialized rpc exception</param>
    /// <returns>RpcError object that contains rpc exception status code and message(detail).</returns>
    public static RpcError ParseRpcErrorMessage(string rpcException)
    {
        var statusCode = string.Empty;
        var detail = string.Empty;

        var matches = Regex.Matches(rpcException, @"(\w+)=""([^""]*)""");

        foreach (Match match in matches)
        {
            if (match.Groups.Count != 3) continue;

            string key = match.Groups[1].Value;
            string value = match.Groups[2].Value;

            if (key.Equals("StatusCode", StringComparison.OrdinalIgnoreCase))
            {
                statusCode = value;
            }
            else if (key.Equals("Detail", StringComparison.OrdinalIgnoreCase))
            {
                detail = value;
            }
        }

        return new RpcError
        {
            StatusCode = statusCode,
            Detail = detail
        };
    }
}