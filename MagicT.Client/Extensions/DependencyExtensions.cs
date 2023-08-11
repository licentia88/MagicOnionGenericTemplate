using MagicOnion.Client;
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

/// <summary>
/// Extension methods for registering client services.
/// </summary>
public static class DependencyExtensions
{
    /// <summary>
    /// Registers client services in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void RegisterClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the local storage service for browser storage.
        services.AddScoped<ILocalStorageService, LocalStorageService>();

        // Register the MagicTClientData to store user data.
        services.AddScoped<MagicTClientData>();
 
        //Register the test service implementation.
        services.AddScoped<ITestService, TestService>();

       

        // Register the Diffie-Hellman key exchange service implementation.
        services.AddScoped<IKeyExchangeService, KeyExchangeService>();

        // Add the TestHub singleton for SignalR communication.
        services.AddSingleton<TestHub>();

        // Add a singleton for a generic list (specify the type later).
        services.AddSingleton(typeof(List<>));

        // Add the RateLimiterFilter singleton for request filtering.
        services.AddSingleton<RateLimiterFilter>();

        // Register Redis database services based on configuration.
        services.RegisterRedisDatabase(configuration);

        // Register the cookie service for handling cookies.
        services.AddScoped<ICookieService, CookieService>();
    }
}