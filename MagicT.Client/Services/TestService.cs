using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// Test service
/// </summary>
public sealed class TestService : MagicClientSecureServiceBase<ITestService, TestModel>, ITestService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public TestService(IServiceProvider provider) : base(provider
        , new RateLimiterFilter(provider), new AuthenticationFilter(provider))
    {
    }
}
