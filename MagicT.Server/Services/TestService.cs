using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public sealed class TestService : MagicServerServiceAuth<ITestService, TestModel, MagicTContextAudit>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider)
    {

    }

 
}

