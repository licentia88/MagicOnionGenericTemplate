using Magic.Server.Database;
using Magic.Server.Services.Base;
using Magic.Shared.Models;
using Magic.Shared.Services;

namespace Magic.Server.Services;

public class TestService : MagicBase<ITestService, TestModel, DummyContext>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider)
    {
    }
}

