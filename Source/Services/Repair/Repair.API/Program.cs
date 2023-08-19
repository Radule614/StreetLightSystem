using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc;
using Common.Notification;
using Microsoft.EntityFrameworkCore;
using Repair.API.Application;
using Repair.API.Domain.Services;
using Repair.API.Domain.Utility;
using Repair.API.Infrastructure;
using Repair.API.Infrastructure.Data;

namespace Repair.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var databaseConfigBuilder = DatabaseConfigBuilder.New();
        var connectionString = databaseConfigBuilder
            .SetFromConfig(builder.Configuration)
            .AddName("RepairDatabase")
            .BuildPostgres();
        builder.Services.AddDbContext<RepairContext>(
            options => options.UseNpgsql(connectionString)
        );
        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor<RepairController>>();
        });
        builder.Services.AddScoped(typeof(RepairRepository));
        builder.Services.AddScoped<IRepairService, RepairService>();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        builder.Services.AddHostedService<StartRepairOrchestrator>();
        builder.Services.AddHostedService<StartRepairSagaHandler>();
        builder.Services.AddHostedService<EndRepairOrchestrator>();
        builder.Services.AddHostedService<EndRepairSagaHandler>();
        builder.Services.AddScoped<IAuthClient, AuthClient>();
        builder.Services.AddScoped<INotificationClient, NotificationClient>();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new RepairProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        var app = builder.Build();
        app.MapGrpcService<RepairController>();
        using (IServiceScope scope = app.Services.CreateScope())
        {
            RepairContext database = scope.ServiceProvider.GetRequiredService<RepairContext>();
            database.Database.Migrate();
        }
        app.Run();
    }
}
