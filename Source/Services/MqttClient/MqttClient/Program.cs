using Common.Gprc;

namespace MqttClient;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddGrpc();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        builder.Services.AddHostedService<PoleClient>();
        var app = builder.Build();
        app.Run();
    }
}
