using MagicT.Client.Filters;
using MagicT.Client.Hubs;
using MagicT.Client.Models;
using MagicT.Client.Services;
using MagicT.Client.Services.JsInterop;
using MagicT.Redis.Extensions;
using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Extensions;

public static class DependencyExtensions
{
    public static void RegisterClientServices(this IServiceCollection Services, IConfiguration configuration)
    {
        //Services.AddBrowserStorage();

        Services.AddScoped<ILocalStorageService,LocalStorageService>();
 
        Services.AddScoped<MagicTUserData>();

        Services.AddScoped<ITestService, TestService>();

        Services.AddScoped<ITokenService, TokenService>();

        Services.AddScoped<IDiffieHellmanKeyExchangeService, DiffieHellmanKeyExchangeService>();


        Services.AddSingleton<TestHub>();

        Services.AddSingleton(typeof(List<>));

        Services.AddSingleton<RateLimiterFilter>();

        Services.RegisterRedisDatabase(configuration);

        Services.AddScoped<ICookieService, CookieService>();
    }
}