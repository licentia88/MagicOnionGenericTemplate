using Benutomo;
using MagicOnion;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Server.Models;
using MagicT.Server.Services.Base;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;
using MessagePipe;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling user roles operations.
/// </summary>
[AutomaticDisposeImpl]
// ReSharper disable once UnusedType.Global
public partial class UserRolesService : MagicServerSecureService<IUserRolesService, USER_ROLES, MagicTContext>, IUserRolesService
{
    /// <summary>
    /// Gets or sets the token publisher.
    /// </summary>
    private IDistributedPublisher<string, EncryptedData<byte[]>> TokenPublisher { get; set; }

    /// <summary>
    /// Gets or sets the MagicT token service.
    /// </summary>
    [EnableAutomaticDispose]
    private TokenManager TokenManager { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRolesService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
        TokenManager = provider.GetService<TokenManager>();
        TokenPublisher = provider.GetService<IDistributedPublisher<string, EncryptedData<byte[]>>>();
    }

    /// <summary>
    /// Creates a new user role.
    /// </summary>
    /// <param name="model">The user role model.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the created <see cref="USER_ROLES"/>.</returns>
    public override async UnaryResult<USER_ROLES> CreateAsync(USER_ROLES model)
    {
        var response = await base.CreateAsync(model);
        await UpdateTokenAsync(model.UR_USER_REFNO);
        return response;
    }

    /// <summary>
    /// Updates an existing user role.
    /// </summary>
    /// <param name="model">The user role model.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the updated <see cref="USER_ROLES"/>.</returns>
    public override async UnaryResult<USER_ROLES> UpdateAsync(USER_ROLES model)
    {
        var response = await base.UpdateAsync(model);
        await UpdateTokenAsync(model.UR_USER_REFNO);
        return response;
    }

    /// <summary>
    /// Deletes an existing user role.
    /// </summary>
    /// <param name="model">The user role model.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing the deleted <see cref="USER_ROLES"/>.</returns>
    public override async UnaryResult<USER_ROLES> DeleteAsync(USER_ROLES model)
    {
        var response = await base.DeleteAsync(model);
        await UpdateTokenAsync(model.UR_USER_REFNO);
        return response;
    }

    /// <summary>
    /// Finds user roles by type.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="roleType">The role type.</param>
    /// <returns>A <see cref="UnaryResult{T}"/> containing a list of <see cref="USER_ROLES"/>.</returns>
    public UnaryResult<List<USER_ROLES>> FindUserRolesByType(int userId, string roleType)
    {
        return ExecuteAsync(async () =>
        {
            var result = await Db.USER_ROLES
                .Where(x => x.AUTHORIZATIONS_BASE.AB_AUTH_TYPE == roleType && x.UR_USER_REFNO == userId)
                .AsNoTracking().ToListAsync();

            return result;
        });
    }

    /// <summary>
    /// Updates the token for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    private async Task UpdateTokenAsync(int userId)
    {
        var roles = await Db.USER_ROLES
            .Where(x => x.UR_USER_REFNO == userId)
            .Select(x => x.UR_ROLE_REFNO).ToArrayAsync();

        var currentCredentials = MagicTRedisDatabase.ReadAs<UsersCredentials>(Convert.ToString(userId));
        
        if(currentCredentials is null) return;
        
        var token = TokenManager.CreateToken(userId, currentCredentials.Identifier, roles);
        var encryptedToken = CryptoHelper.EncryptData(token, currentCredentials.SharedKey);
        await TokenPublisher.PublishAsync(currentCredentials.Identifier.ToUpper(), encryptedToken);
    }
}