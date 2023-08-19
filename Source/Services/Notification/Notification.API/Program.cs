using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc;
using Microsoft.EntityFrameworkCore;
using Notification.API.Application;
using Notification.API.Domain.Services;
using Notification.API.Domain.Utility;
using Notification.API.Infrastructure;
using Notification.API.Infrastructure.Data;

namespace Notification.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var databaseConfigBuilder = DatabaseConfigBuilder.New();
        var connectionString = databaseConfigBuilder
            .SetFromConfig(builder.Configuration)
            .AddName("NotificationDatabase")
            .BuildPostgres();
        builder.Services.AddDbContext<NotificationContext>(
            options => options.UseNpgsql(connectionString)
        );
        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor<NotificationController>>();
        });
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        builder.Services.AddScoped(typeof(NotificationRepository));
        builder.Services.AddScoped<IAuthClient, AuthClient>();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new NotificationProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        var app = builder.Build();
        app.MapGrpcService<NotificationController>();
        using (IServiceScope scope = app.Services.CreateScope())
        {
            NotificationContext database = scope.ServiceProvider.GetRequiredService<NotificationContext>();
            database.Database.Migrate();
        }
        app.Run();
    }
}
