using Benutomo;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Jwt;
using MagicT.Server.Models;
using MagicT.Server.Services.Base;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Services;
using MessagePipe;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

[AutomaticDisposeImpl]
public partial class UserRolesService : MagicServerSecureService<IUserRolesService, USER_ROLES, MagicTContext>, IUserRolesService, IDisposable, IAsyncDisposable
{
    public IDistributedPublisher<string, EncryptedData<byte[]>> TokenPublisher { get; set; }

    [EnableAutomaticDispose]
    public MagicTTokenService MagicTTokenService { get; set; }

    public UserRolesService(IServiceProvider provider) : base(provider)
    {
        MagicTTokenService = provider.GetService<MagicTTokenService>();
        TokenPublisher = provider.GetService<IDistributedPublisher<string, EncryptedData<byte[]>>>();
    }

   

    public override async UnaryResult<USER_ROLES> CreateAsync(USER_ROLES model)
    {
        var response = await  base.CreateAsync(model);
        var roles = await Db.USER_ROLES
                          .Where(x => x.UR_USER_REFNO == model.UR_USER_REFNO)
                          .Select(x => x.UR_ROLE_REFNO).ToArrayAsync();

        var currentCredentials = MagicTRedisDatabase.ReadAs<UsersCredentials>(Convert.ToString(model.UR_USER_REFNO));
        var token = MagicTTokenService.CreateToken(model.UR_USER_REFNO, currentCredentials.Identifier, roles);
        var encryptedToken = CryptoHelper.EncryptData(token, currentCredentials.SharedKey);
        await  TokenPublisher.PublishAsync(currentCredentials.Identifier.ToUpper(), encryptedToken);

        return response;
    }

    public override async UnaryResult<USER_ROLES> UpdateAsync(USER_ROLES model)
    {
        var response = await base.UpdateAsync(model);

        var roles = await Db.USER_ROLES
                          .Where(x => x.UR_USER_REFNO == model.UR_USER_REFNO)
                          .Select(x => x.UR_ROLE_REFNO).ToArrayAsync();

        var currentCredentials = MagicTRedisDatabase.ReadAs<UsersCredentials>(Convert.ToString(model.UR_USER_REFNO));
        var token = MagicTTokenService.CreateToken(model.UR_USER_REFNO, currentCredentials.Identifier, roles);
        var encryptedToken = CryptoHelper.EncryptData(token, currentCredentials.SharedKey);
        await TokenPublisher.PublishAsync(currentCredentials.Identifier.ToUpper(), encryptedToken);
        return response;
    }

    public override async UnaryResult<USER_ROLES> DeleteAsync(USER_ROLES model)
    {
        var response = await base.DeleteAsync(model);

        var roles = await Db.USER_ROLES
                          .Where(x=> x.UR_USER_REFNO == model.UR_USER_REFNO)
                          .Select(x => x.UR_ROLE_REFNO).ToArrayAsync();

        var test = await Db.USER_ROLES.Where(x => x.UR_ROLE_REFNO == model.UR_ROLE_REFNO).ToListAsync();

        var currentCredentials = MagicTRedisDatabase.ReadAs<UsersCredentials>(Convert.ToString(model.UR_USER_REFNO));
        var token = MagicTTokenService.CreateToken(model.UR_USER_REFNO, currentCredentials.Identifier, roles);
        var userShared = MagicTRedisDatabase.ReadAs<UsersCredentials>(Convert.ToString(model.UR_USER_REFNO)).SharedKey;
        var encryptedToken = CryptoHelper.EncryptData(token, currentCredentials.SharedKey);
        await TokenPublisher.PublishAsync(currentCredentials.Identifier.ToUpper(), encryptedToken);
        return response;
    }

    

    public UnaryResult<List<USER_ROLES>> FindUserRolesByType(int userId, string RoleType)
    {
        return ExecuteAsync(async () =>
        {
            return await Db.USER_ROLES
                       .Where(x => x.AUTHORIZATIONS_BASE.AB_AUTH_TYPE == RoleType && x.UR_USER_REFNO == userId)
                       .AsNoTracking().ToListAsync();
        });
    }
}
