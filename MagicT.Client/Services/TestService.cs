using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

public class TestService : ServiceBase<ITestService, TestModel>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider, new AuthenticationFilter(provider))
    {
    }
}
