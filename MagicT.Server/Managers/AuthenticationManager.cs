using Grpc.Core;
using MagicOnion;
using MagicT.Redis;
using MagicT.Server.Jwt;
using MagicT.Server.Models;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Managers;

public class AuthenticationManager
{
    private MagicTRedisDatabase MagicTRedisDatabase { get; set; }


    public Lazy<List<PERMISSIONS>> PermissionList { get; set; }


    public AuthenticationManager(IServiceProvider provider)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
        PermissionList = provider.GetService<Lazy<List<PERMISSIONS>>>();

    }

    /// <summary>
    ///  Validates the roles of the token against the required roles for the service.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="encryptedBytes"></param>
    /// <param name="nonce"></param>
    /// <param name="mac"></param>
    /// <exception cref="ReturnStatusException"></exception>
    public void AuthenticateData(int id, EncryptedData<AuthenticationData> authenticationData)
    {
        var key = Convert.ToString(id);

        var usedTokens = MagicTRedisDatabase.PullAs<UsedTokens>(key);

        var expiredToken = usedTokens.FirstOrDefault(x => 
                        x.EncryptedBytes == authenticationData.EncryptedBytes &&
                        x.Nonce == authenticationData.Nonce &&
                        x.Mac == authenticationData.Mac);
 
        if (expiredToken is not null)
            throw new ReturnStatusException(StatusCode.Unauthenticated, "Expired Token");

        var currentToken = new UsedTokens(authenticationData.EncryptedBytes, authenticationData.Nonce, authenticationData.Mac);


        MagicTRedisDatabase.Push(key, currentToken);
        
    }

    public void ValidateRoles(MagicTToken token, string endPoint)
    {
        var permission = PermissionList.Value.Find(x => x.PER_PERMISSION_NAME == endPoint);

        if (permission is null)
            throw new ReturnStatusException(StatusCode.Unauthenticated, "Permission not implemented");

        //User permission should match either role or permission itself
        if (!token.Roles.Any(x => x == permission.AB_ROWID || x == permission.PER_ROLE_REFNO))
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));


    }
}
