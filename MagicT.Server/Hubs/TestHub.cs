using Benutomo;
using MagicOnion.Server.Hubs;
using MagicT.Server.Database;
using MagicT.Server.Hubs.Base;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;

namespace MagicT.Server.Hubs;

public sealed  class TestHub : MagicHubServerBase<ITestHub, ITestHubReceiver, TestModel, MagicTContext>, ITestHub
{
    public TestHub(IServiceProvider provider) : base(provider) { }
}

 