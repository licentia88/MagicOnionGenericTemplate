using Benutomo;
using Grpc.Core;
using MagicOnion;
using MagicT.Redis;
using MagicT.Server.Jwt;
using MagicT.Server.Models;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Managers;

/// <summary>
/// Manages authentication operations including validating roles and authenticating data.
/// </summary>
[AutomaticDisposeImpl]
public partial class AuthenticationManager : IDisposable
{
    [EnableAutomaticDispose]
    private MagicTRedisDatabase MagicTRedisDatabase { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationManager"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthenticationManager(IServiceProvider provider)
    {
        MagicTRedisDatabase = provider.GetService<MagicTRedisDatabase>();
    }

    ~AuthenticationManager()
    {
        Dispose(false);
    }
    /// <summary>
    /// Authenticates the data by validating the token against used tokens in the Redis database.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <param name="authenticationData">The authentication data to validate.</param>
    /// <exception cref="ReturnStatusException">Thrown when the token is expired.</exception>
    public void AuthenticateData(int id, EncryptedData<AuthenticationData> authenticationData)
    {
        var key = Convert.ToString(id);
        var usedTokens = MagicTRedisDatabase.PullAs<UsedTokens>(key);

        if (IsTokenExpired(usedTokens, authenticationData))
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, "Expired Token");
        }

        var currentToken = new UsedTokens(authenticationData.EncryptedBytes, authenticationData.Nonce, authenticationData.Mac);
        MagicTRedisDatabase.Push(key, currentToken);
    }

    /// <summary>
    /// Validates the roles of the token against the required roles for the service.
    /// </summary>
    /// <param name="token">The MagicT token containing user roles.</param>
    /// <param name="endPoint">The endpoint to validate against.</param>
    /// <exception cref="ReturnStatusException">Thrown when the permission is not implemented or the user does not have the required roles.</exception>
    public void ValidateRoles(MagicTToken token, string endPoint)
    {
        var permission = MagicTRedisDatabase.ReadAs<PERMISSIONS>(endPoint);

        if (permission is null)
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, "Permission not implemented");
        }

        if (!HasRequiredRole(token, permission))
        {
            throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
        }
    }

    /// <summary>
    /// Checks if the token is expired by comparing it with used tokens.
    /// </summary>
    /// <param name="usedTokens">The list of used tokens.</param>
    /// <param name="authenticationData">The authentication data to validate.</param>
    /// <returns>True if the token is expired, otherwise false.</returns>
    private bool IsTokenExpired(IEnumerable<UsedTokens> usedTokens, EncryptedData<AuthenticationData> authenticationData)
    {
        return usedTokens.Any(x =>
            x.EncryptedBytes == authenticationData.EncryptedBytes &&
            x.Nonce == authenticationData.Nonce &&
            x.Mac == authenticationData.Mac);
    }

    /// <summary>
    /// Checks if the token has the required role.
    /// </summary>
    /// <param name="token">The MagicT token containing user roles.</param>
    /// <param name="permission">The required permission.</param>
    /// <returns>True if the token has the required role, otherwise false.</returns>
    private bool HasRequiredRole(MagicTToken token, PERMISSIONS permission)
    {
        return token.Roles.Any(x => x == permission.AB_ROWID || x == permission.PER_ROLE_REFNO);
    }
}