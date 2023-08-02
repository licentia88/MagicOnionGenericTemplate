using MagicT.Server.Database;
using MagicT.Server.Filters;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace MagicT.Server.Services;

[RateLimiter(1, 1)]
public class TestService : MagicBase<ITestService, TestModel, MagicTContext>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider)
    {
    }
}

