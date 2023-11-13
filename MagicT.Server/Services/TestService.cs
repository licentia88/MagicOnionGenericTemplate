using System.Text;
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

    public async UnaryResult<string> EncryptedString(EncryptedData<string> data)
    {
        await Task.Delay(1);
        var sharedKeyString = ASCIIEncoding.UTF8.GetString(globalData.SharedBytes);  

  
 
        var sharedKey = GetSharedKey(Context);
        var sharedKeyString2 = ASCIIEncoding.UTF8.GetString(sharedKey);

        var decryptedData = string.Empty;
        try
        {
             decryptedData = CryptoHelper.DecryptData(data, sharedKey);
        }
        catch (Exception ex)
        {

        }


        try
        {
             decryptedData = CryptoHelper.DecryptData(data, globalData.SharedBytes);
        }
        catch (Exception ex)
        {

        }

        return decryptedData;
     }
}
