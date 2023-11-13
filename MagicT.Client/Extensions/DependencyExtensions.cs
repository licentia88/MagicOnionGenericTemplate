using Blazored.LocalStorage;
using MagicT.Client.Hubs;
using MagicT.Client.Models;
using MagicT.Client.Services;
using MagicT.Client.Services.JsInterop;
using MagicT.Redis.Extensions;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;
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
        services.AddSingleton<KeyExchangeData>();

        services.AddSingleton(typeof(List<>));

        services.AddScoped<MagicTClientData>();

         services.AddBlazoredLocalStorage();

        //services.AddScoped<ILocalStorageService, LocalStorageService>();

        RegisterHubsAndServices(services);
        
        // Register Redis database services based on configuration.
        services.RegisterRedisDatabase(configuration);

        // Register the cookie service for handling cookies.
        services.AddScoped<ICookieService, CookieService>();
    }
    private static void RegisterHubsAndServices(this IServiceCollection services)
    {
        //Register the test service implementation.
        services.AddScoped<ITestService, TestService>();

        //Register the UserService service implementation.
        services.AddScoped<IUserService, UserService>();


        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<IUserRolesService, UserRolesService>();

        services.AddScoped<IRolesService, RolesService>();
        
        // services.AddScoped<IRolesDService, RolesDService>();

        services.AddScoped<IPermissionsService, PermissionsService>();

        // Register the Diffie-Hellman key exchange service implementation.
        services.AddScoped<KeyExchangeService>();

        // Add the TestHub singleton for SignalR communication.
        services.AddSingleton<TestHub>();
    }
}