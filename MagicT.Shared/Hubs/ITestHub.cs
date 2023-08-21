using MagicT.Client.Models;
using MagicT.Shared.Hubs.Base;
using MagicT.Shared.Models;

namespace MagicT.Shared.Hubs;

public interface ITestHub : IMagicTHub<ITestHub, ITestHubReceiver, TestModel>
{
}

public interface ITestHubReceiver : IMagicTReceiver<TestModel>
{
}


