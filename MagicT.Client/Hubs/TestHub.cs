using MagicT.Client.Hubs.Base;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;

namespace MagicT.Client.Hubs;

public class TestHub : MagicHubClientBase<ITestHub, ITestHubReceiver, TestModel>, ITestHubReceiver
{
    public TestHub(IServiceProvider provider) : base(provider)
    {
    }
}