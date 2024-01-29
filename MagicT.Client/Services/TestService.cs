using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;


/// <summary>
/// Test service
/// </summary>
[RegisterScoped]
public sealed class TestService : MagicClientService<ITestService, TestModel>, ITestService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public TestService(IServiceProvider provider)
        : base(provider)
    {
    }

    public UnaryResult CreateMillionData()
    {
        return Client.CreateMillionData();
    }
}
