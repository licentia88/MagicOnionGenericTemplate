using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditRecordsService : MagicServerAuthService<IAuditRecordsService, AUDIT_RECORDS,MagicTContext>, IAuditRecordsService
{
    public AuditRecordsService(IServiceProvider provider) : base(provider)
    {
    }
}

public class SecureTestService : MagicServerAuthService<ISecureTestService, TestModel, MagicTContext>, ISecureTestService
{
    public SecureTestService(IServiceProvider provider) : base(provider)
    {
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
