using Grpc.Core;

namespace Common.Gprc.Exceptions;

/// <summary>
/// Not found rpc exception used by gprc services to express runtime errors
/// </summary>
public class EntityNotFoundException : RpcException
{
    /// <param name="entityName">Name of the entity that has not been found.</param>
    /// <param name="entityId">Id of the entity that has not been found.</param>
    protected EntityNotFoundException(string entityName, Guid entityId) :
        base(new Status(StatusCode.NotFound, $"{entityName} with ID: {entityId} has not been found"))
    { }

    /// <param name="entityName">Name of the entity that has not been found.</param>
    /// <param name="propertyName">Property name of the entity that has not been found.</param>
    /// <param name="propertyValue">Property value.</param>
    protected EntityNotFoundException(string entityName, string propertyName, string propertyValue) :
        base(new Status(StatusCode.NotFound, $"{entityName} with ${propertyName}: {propertyValue} has not been found"))
    { }
}
