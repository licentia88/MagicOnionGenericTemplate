using AQueryMaker.Extensions;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Services.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
using Mapster;

namespace MagicT.Server.Services;
public class AuditQueryService : MagicServerServiceAuth<IAuditQueryService, AUDIT_QUERY,MagicTContext>, IAuditQueryService
{
    public AuditQueryService(IServiceProvider provider) : base(provider)
    {
    }

 
}