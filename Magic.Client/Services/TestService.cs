using Magic.Client.Services.Base;
using Magic.Shared.Models;
using Magic.Shared.Services;

namespace Magic.Client.Services;

[RegisterSingleton]
public class TestService : ServiceBase<ITestService, TestModel>, ITestService
{
    public TestService()
    {
    }

}

