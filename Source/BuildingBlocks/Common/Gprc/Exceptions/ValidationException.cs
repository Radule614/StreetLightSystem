using Grpc.Core;

namespace Common.Gprc.Exceptions;
/// <summary>
/// Validation rpc exception used by gprc services to express validation errors.
/// </summary>
public class ValidationException : RpcException
{
    /// <summary>
    /// Constructor for creating ValidationException used for property validation purposes.
    /// </summary>
    /// <param name="propertyName">Name of the property that has failed validation.</param>
    /// <param name="validationMessage">Validation error message.</param>
    public ValidationException(string propertyName, string validationMessage) :
        base(new Status(
            StatusCode.InvalidArgument,
            $"{propertyName}: ${validationMessage}"))
    { }

    /// <summary>
    /// Constructor used by ValidationExceptionBuilder.
    /// </summary>
    public ValidationException(string errors) :
        base(new Status(
            StatusCode.InvalidArgument, errors))
    { }
}
