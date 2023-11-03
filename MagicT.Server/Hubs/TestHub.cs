using MagicT.Server.Database;
using MagicT.Server.Hubs.Base;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;
using MessagePipe;

namespace MagicT.Server.Hubs;

//[MagicAuthorize]
public sealed class TestHub : MagicHubServerBase<ITestHub, ITestHubReceiver, TestModel, MagicTContext>, ITestHub
{
    public TestHub(IServiceProvider provider) : base(provider) { }
}
