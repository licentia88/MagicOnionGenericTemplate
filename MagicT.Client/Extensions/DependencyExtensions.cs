using MagicT.Client.Filters;
using MagicT.Client.Hubs;
using MagicT.Client.Models;
using MagicT.Client.Services;
using MagicT.Redis.Extensions;
using MagicT.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Extensions;

public static class DependencyExtensions
{
    public static void RegisterClientServices(this IServiceCollection Services,IConfiguration configuration)
    {
        Services.AddScoped<MagicTUserData>();

        Services.AddSingleton<RateLimiterFilter>();

        Services.AddSingleton<ITestService, TestService>();


        Services.AddSingleton<ITokenService, TokenService>();
        Services.AddSingleton<TestHub>();
        Services.AddSingleton(typeof(List<>));
        Services.RegisterRedisDatabase(configuration);
    }
}

