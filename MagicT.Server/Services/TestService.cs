using MagicT.Server.Database;
using MagicT.Server.Filters;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

[MagicTAuthorize]
// ReSharper disable once UnusedType.Global
public sealed class TestService : MagicServerServiceBase<ITestService, TestModel, MagicTContext>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider)
    {
    }
}
