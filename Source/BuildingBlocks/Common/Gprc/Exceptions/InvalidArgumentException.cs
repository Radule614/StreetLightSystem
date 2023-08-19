using Grpc.Core;

namespace Common.Gprc.Exceptions;

/// <summary>
/// Invalid argument rpc exception used by gprc services to express runtime errors.
/// </summary>
public class InvalidArgumentException : RpcException
{
    /// <summary>
    /// Constructor for creating detailed InvalidArgumentException.
    /// </summary>
    /// <param name="argumentName">Name of the argument that's invalid</param>
    /// <param name="argumentValue">Argument value that's invalid</param>
    /// <param name="properArgumentFormat">Proper argument format</param>
    public InvalidArgumentException(string argumentName, string argumentValue, string properArgumentFormat) :
        base(new Status(
            StatusCode.InvalidArgument,
            $"Invalid argument specified for parameter {argumentName}. Specified value: {argumentValue}. Proper format: {properArgumentFormat}"))
    { }
}
