using Grpc.Net.Client;

namespace Common.Gprc;

/// <summary>
/// Interface for channel factory dependency injection
/// </summary>
public interface IChannelFactory
{
    /// <summary>
    /// Creates gRPC channel for the given address, gRPC channel is used for gRPC client connection
    /// </summary>
    /// <param name="address">Address used to build grpc channel (examples -> localhost:12000, pole_service:12000)</param>
    /// <returns>Channel used for gRPC client connection</returns>
    public GrpcChannel GetChannel(string address);
}