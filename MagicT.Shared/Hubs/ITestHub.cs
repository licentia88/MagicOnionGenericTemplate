using MagicT.Shared.Hubs.Base;
using MagicT.Shared.Models;

namespace MagicT.Shared.Hubs;

public interface ITestHub:IMagicHub<ITestHub,ITestHubReceiver,TestModel>
{

}

public interface ITestHubReceiver:IMagicReceiver<TestModel>
{

}

