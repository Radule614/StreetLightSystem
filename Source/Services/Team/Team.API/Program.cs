using Common.Auth;
using Common;
using Microsoft.EntityFrameworkCore;
using Team.API.Application;
using Team.API.Infrastructure.Data;
using AutoMapper;
using Common.Gprc;
using Team.API.Domain.Services;
using Team.API.Domain.Utility;
using Team.API.Infrastructure;
using Common.Notification;

namespace Team.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var databaseConfigBuilder = DatabaseConfigBuilder.New();
        var connectionString = databaseConfigBuilder
            .SetFromConfig(builder.Configuration)
            .AddName("TeamDatabase")
            .BuildPostgres();
        builder.Services.AddDbContext<TeamContext>(
            options => options.UseNpgsql(connectionString)
        );
        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor<TeamController>>();
        });
        builder.Services.AddScoped(typeof(Repository<>));
        builder.Services.AddScoped<ITeamService, TeamService>();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        builder.Services.AddHostedService<CreateUserSagaHandler>();
        builder.Services.AddHostedService<DeleteUserSagaHandler>();
        builder.Services.AddHostedService<UpdateUserSagaHandler>();
        builder.Services.AddHostedService<StartRepairSagaHandler>();
        builder.Services.AddHostedService<EndRepairSagaHandler>();
        builder.Services.AddScoped<IAuthClient, AuthClient>();
        builder.Services.AddScoped<INotificationClient, NotificationClient>();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new TeamProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        var app = builder.Build();
        app.MapGrpcService<TeamController>();
        using (IServiceScope scope = app.Services.CreateScope())
        {
            TeamContext database = scope.ServiceProvider.GetRequiredService<TeamContext>();
            database.Database.Migrate();
        }
        app.Run();
    }
}
