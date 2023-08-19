using Grpc.Net.Client;

namespace Common.Gprc;

/// <summary>
/// Factory class responsible for creating channel objects which are used for gRPC client connections.
/// </summary>
public class ChannelFactory : IChannelFactory
{   
    /// <inheritdoc />
    public GrpcChannel GetChannel(string? address)
    {
        if (address == null)
        {
            throw new ArgumentException($"Invalid address specified {address}");
        }
        return GrpcChannel.ForAddress("http://" + address);
    }
}