using Magic.Server.Database;
using Magic.Server.Hubs.Base;
using Magic.Shared.Hubs;
using Magic.Shared.Models;

namespace Magic.Server.Hubs;

//[MagicAuthorize]
public class TestHub : MagicHubServerBase<ITestHub, ITestHubReceiver, TestModel, DummyContext>,ITestHub
{
    public TestHub(IServiceProvider provider) : base(provider)
    {
    }
}

