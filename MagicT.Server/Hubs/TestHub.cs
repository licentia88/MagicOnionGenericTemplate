using MagicT.Server.Database;
using MagicT.Server.Hubs.Base;
using MagicT.Shared.Hubs;
using MagicT.Shared.Models;
// ReSharper disable UnusedType.Global

namespace MagicT.Server.Hubs;

/// <summary>
///  A hub class that provides CRUD operations for the TestModel model.
/// </summary>
public sealed  class TestHub : MagicHubDataBase<ITestHub, ITestHubReceiver,TestModel, MagicTContext>, ITestHub
{
    public TestHub(IServiceProvider provider) : base(provider) { }
}

 