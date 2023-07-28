using Magic.Client.Hubs;
using Magic.Client.Services;
using Magic.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Magic.Client.Extensions;

public static class DependencyExtensions
{
    public static void RegisterClientServices(this IServiceCollection Services)
    {
        Services.AddSingleton<ITestService, TestService>();
        Services.AddSingleton<ITokenService, TokenService>();
        Services.AddSingleton<TestHub>();
        Services.AddSingleton(typeof(List<>));
    }
}

