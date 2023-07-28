using Magic.Shared.Hubs.Base;
using Magic.Shared.Models;

namespace Magic.Shared.Hubs;

public interface ITestHub:IMagicHub<ITestHub,ITestHubReceiver,TestModel>
{

}

public interface ITestHubReceiver:IMagicReceiver<TestModel>
{

}

