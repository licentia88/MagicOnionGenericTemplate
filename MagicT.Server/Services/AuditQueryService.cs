using System.Data;
using AQueryMaker.Extensions;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;
//using MapDataReader;

namespace MagicT.Server.Services;

 
public class AuditQueryService : MagicServerAuthService<IAuditQueryService, AUDIT_QUERY,MagicTContext>, IAuditQueryService
{
    public AuditQueryService(IServiceProvider provider) : base(provider)
    {
    }

    public override async UnaryResult<List<AUDIT_QUERY>> FindByParametersAsync(byte[] parameters)
    {

        //return await base.FindByParametersAsync(parameters);

        var queryData = QueryManager.BuildQuery<AUDIT_QUERY>(parameters);

        var reader = await Db.Manager().QueryReaderAsync(queryData.query, queryData.parameters);


        List<AUDIT_QUERY> results = reader.ToAUDIT_QUERY();

        return results;
    }

}