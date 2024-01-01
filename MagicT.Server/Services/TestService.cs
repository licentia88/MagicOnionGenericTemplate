using Benutomo;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

[AutomaticDisposeImpl]
public sealed partial class TestService : MagicServerAuthService<ITestService, TestModel, MagicTContext>, ITestService, IDisposable, IAsyncDisposable
{
    public KeyExchangeData globalData { get; set; }

    public TestService(IServiceProvider provider) : base(provider)
    {
        globalData = provider.GetService<KeyExchangeData>();
    }

    public UnaryResult<string> EncryptedString(EncryptedData<string> data)
    {

        var sharedKey = SharedKey;

        var decryptedData = string.Empty;
        try
        {
            decryptedData = CryptoHelper.DecryptData(data, sharedKey);
        }
        catch
        {

        }
 
        return UnaryResult.FromResult(decryptedData);
    }
}
