using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public sealed class TestService : MagicServerServiceAuth<ITestService, TestModel, MagicTContext>, ITestService
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
