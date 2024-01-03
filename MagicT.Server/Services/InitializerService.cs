using Benutomo;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Enums;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

[AutomaticDisposeImpl]
public partial class InitializerService : MagicServerBase<IInitializerService>, IInitializerService,IAsyncDisposable,IDisposable
{
    [EnableAutomaticDispose]
    public MagicTContext Db { get; set; }

    public InitializerService(IServiceProvider provider) : base(provider)
    {
        Db = provider.GetService<MagicTContext>();
    }

    public UnaryResult<List<PERMISSIONS>> GetPermissions()
    {
        return ExecuteAsync( async () =>
        {
            return await Db.PERMISSIONS.ToListAsync() ;
        });
    }

    public UnaryResult<List<ROLES>> GetRoles()
    {
        return ExecuteAsync(async () =>
        {
            return await Db.ROLES.ToListAsync();
        });
    }

    public UnaryResult<List<USERS>> GetUsers()
    {
        return ExecuteAsync(async () =>
        {
            return await Db.USERS.ToListAsync();
        });
    }

    public UnaryResult<List<Operations>> GetOperations()
    {
        return Execute(() =>
        {
            var values = Enum.GetValues<AuditType>();

            return values.Select((op, val) => new Operations { Id = val, Description = op.ToString() }).ToList();

        });
    }
}