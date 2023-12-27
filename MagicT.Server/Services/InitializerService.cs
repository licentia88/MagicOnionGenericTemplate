using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Enums;
using MagicT.Server.Services.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

public class InitializerService : MagicServerBase<IInitializerService>, IInitializerService
{
    public MagicTContext Db { get; set; }

    public InitializerService(IServiceProvider provider) : base(provider)
    {
        Db = provider.GetService<MagicTContext>();
    }

    public UnaryResult<List<PERMISSIONS>> GetPermissions()
    {
        return ExecuteWithoutResponseAsync( async () =>
        {
            return await Db.PERMISSIONS.ToListAsync() ;
        });
    }

    public UnaryResult<List<ROLES>> GetRoles()
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            return await Db.ROLES.ToListAsync();
        });
    }

    public UnaryResult<List<USERS>> GetUsers()
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            return await Db.USERS.ToListAsync();
        });
    }

    public UnaryResult<List<Operations>> GetOperations()
    {
        return ExecuteWithoutResponse(() =>
        {
            var values = Enum.GetValues<AuditType>();

            return values.Select((AuditType op, int val) => new Operations { Id = val, Description = op.ToString() }).ToList();

        });
    }
}