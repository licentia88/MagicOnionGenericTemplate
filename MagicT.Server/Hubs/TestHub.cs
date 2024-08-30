using MagicT.Server.Database;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;

namespace MagicT.Server.Hubs;

public sealed  class TestHub : Base.MagicHubServerBase<ITestHub, ITestHubReceiver,TestModel, MagicTContext>, ITestHub
{
    public TestHub(IServiceProvider provider) : base(provider) { }
}

 