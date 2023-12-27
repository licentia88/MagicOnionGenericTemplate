using MagicT.Client.Hubs.Base;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;

namespace MagicT.Client.Hubs;

/// <summary>
/// Test hub
/// </summary>
[RegisterSingleton]
public sealed class TestHub : MagicHubClientBase<ITestHub, ITestHubReceiver, TestModel>, ITestHubReceiver, ITestHub
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public TestHub(IServiceProvider provider) : base(provider)
    {
    }

    public Task CollectionChanged()
    {
        return Client.CollectionChanged();
    }

    public Task DisposeAsync()
    {
        return Client.DisposeAsync();
    }

    public ITestHub FireAndForget()
    {
        return Client.FireAndForget();
    }

    public Task WaitForDisconnect()
    {
        return Client.WaitForDisconnect();
    }
}
