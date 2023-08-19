using Auth.API.Application;
using Auth.API.Services;
using Common;
using Common.Gprc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Auth.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddGrpc();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IChannelFactory, ChannelFactory>();
        Bearer.Configuration = builder.Configuration;
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Bearer.BearerOptions);
        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGrpcService<AuthController>();
        app.Run();
    }
}
