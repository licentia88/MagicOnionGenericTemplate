using MagicT.Client.Hubs;
using MagicT.Client.Services;
using MagicT.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Extensions;

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

