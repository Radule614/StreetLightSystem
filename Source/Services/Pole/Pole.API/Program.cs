using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc;
using Common.Notification;
using Microsoft.EntityFrameworkCore;
using Pole.API.Application;
using Pole.API.Domain.Services;
using Pole.API.Domain.Utility;
using Pole.API.Infrastructure;
using Pole.API.Infrastructure.Data;

namespace Pole.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var databaseConfigBuilder = DatabaseConfigBuilder.New();
        var connectionString = databaseConfigBuilder
            .SetFromConfig(builder.Configuration)
            .AddName("PoleDatabase")
            .BuildPostgres();
        builder.Services.AddDbContext<PoleContext>(
            options => options.UseNpgsql(connectionString)
        );
        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor<PoleController>>();
        });
        builder.Services.AddScoped(typeof(PoleRepository));
        builder.Services.AddScoped<IPoleService, PoleService>();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        builder.Services.AddHostedService<StartRepairSagaHandler>();
        builder.Services.AddHostedService<EndRepairSagaHandler>();
        builder.Services.AddScoped<IAuthClient, AuthClient>();
        builder.Services.AddScoped<INotificationClient, NotificationClient>();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new PoleProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        var app = builder.Build();
        app.MapGrpcService<PoleController>();
        using (IServiceScope scope = app.Services.CreateScope())
        {
            PoleContext database = scope.ServiceProvider.GetRequiredService<PoleContext>();
            database.Database.Migrate();
        }
        app.Run();
    }
}
