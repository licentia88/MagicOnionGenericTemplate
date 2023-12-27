using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;


/// <summary>
/// Test service
/// </summary>
[RegisterScoped]
public sealed class TestService : MagicClientSecureService<ITestService, TestModel>, ITestService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public TestService(IServiceProvider provider)
        : base(provider)
    {
    }

    public UnaryResult<string> EncryptedString(EncryptedData<string> data)
    {
        return Client.EncryptedString(data);
    }
}
