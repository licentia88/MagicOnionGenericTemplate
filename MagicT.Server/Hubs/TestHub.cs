using MagicT.Server.Database;
using MagicT.Server.Hubs.Base;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;

namespace MagicT.Server.Hubs;

//[MagicAuthorize]
public class TestHub : MagicHubServerBase<ITestHub, ITestHubReceiver, TestModel, MagicTContext>,ITestHub
{
    public TestHub(IServiceProvider provider) : base(provider)
    {
    }
}

