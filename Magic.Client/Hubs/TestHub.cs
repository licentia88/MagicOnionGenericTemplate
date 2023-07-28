using Magic.Client.Hubs.Base;
using Magic.Shared.Hubs;
using Magic.Shared.Models;

namespace Magic.Client.Hubs;

public class TestHub : MagicHubClientBase<ITestHub, ITestHubReceiver, TestModel>, ITestHubReceiver
{
    public TestHub(IServiceProvider provider) : base(provider)
    {
    }
}

