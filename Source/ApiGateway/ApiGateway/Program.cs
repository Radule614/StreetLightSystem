using ApiGateway.Middleware;
using ApiGateway.Services;
using Common.Gprc;

namespace ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        builder.Services.AddSignalR();
        builder.Services.AddGrpc();

        var app = builder.Build();
        app.UseExceptionHandler("/Error");
        app.UseHttpsRedirection();
        app.UseRouting();
        app.MapGrpcService<NotificationService>().RequireHost("*:12000");
        app.UseCors(corsBuilder =>
        {
            corsBuilder
                .WithOrigins("http://localhost:3000", "http://192.168.1.2:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
        app.UseMiddleware<MetadataMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<NotificationHub>("/api/notification");
        app.Run();
    }
}