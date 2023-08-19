using AutoMapper;
using Common;
using Common.Auth;
using Common.Gprc;
using Common.Notification;
using Microsoft.EntityFrameworkCore;
using User.API.Application;
using User.API.Domain.Services;
using User.API.Domain.Utility;
using User.API.Infrastructure;
using User.API.Infrastructure.Data;

namespace User.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var databaseConfigBuilder = DatabaseConfigBuilder.New();
        var connectionString = databaseConfigBuilder
            .SetFromConfig(builder.Configuration)
            .AddName("UserDatabase")
            .BuildPostgres();
        builder.Services.AddDbContext<UserContext>(
            options => options.UseNpgsql(connectionString)
        );
        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<AuthInterceptor<UserController>>();
        });
        builder.Services.AddScoped(typeof(Repository<>));
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        builder.Services.AddHostedService<CreateUserOrchestrator>();
        builder.Services.AddHostedService<CreateUserSagaHandler>();
        builder.Services.AddHostedService<DeleteUserOrchestrator>();
        builder.Services.AddHostedService<DeleteUserSagaHandler>();
        builder.Services.AddHostedService<UpdateUserOrchestrator>();
        builder.Services.AddHostedService<UpdateUserSagaHandler>();
        builder.Services.AddScoped<IAuthClient, AuthClient>();
        builder.Services.AddScoped<INotificationClient, NotificationClient>();
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new UserProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        var app = builder.Build();
        app.MapGrpcService<UserController>();
        using (IServiceScope scope = app.Services.CreateScope())
        {
            UserContext database = scope.ServiceProvider.GetRequiredService<UserContext>();
            database.Database.Migrate();
        }
        app.Run();
    }
}
