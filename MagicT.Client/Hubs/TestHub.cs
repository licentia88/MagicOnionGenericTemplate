using MagicT.Client.Hubs.Base;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;

namespace MagicT.Client.Hubs;

/// <summary>
/// Test hub
/// </summary>
public sealed class TestHub : MagicHubClientBase<ITestHub, ITestHubReceiver, TestModel>, ITestHubReceiver
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public TestHub(IServiceProvider provider) : base(provider)
    {
    }

   
}
