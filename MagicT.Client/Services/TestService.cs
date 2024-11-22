using Benutomo;
using MagicOnion;
using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;


/// <summary>
/// Test service
/// </summary>
[RegisterScoped]
[AutomaticDisposeImpl]
public partial class TestService : MagicClientService<ITestService, TestModel>, ITestService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public TestService(IServiceProvider provider)
        : base(provider, new RateLimiterFilter(provider))
    {
    }

    ~TestService()
    {
        Dispose(false);
    }
    public UnaryResult CreateMillionsData()
    {
        return Client.CreateMillionsData();
    }

    
}
