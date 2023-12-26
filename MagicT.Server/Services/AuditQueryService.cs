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

    public override UnaryResult<List<AUDIT_QUERY>> FindByParametersAsync(byte[] parameters)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            var dictionary = parameters.UnPickleFromBytes<KeyValuePair<string, object>[]>();

            var whereStatement = $" WHERE {string.Join(" AND ", dictionary.Select(x => $" {x.Key} = @{x.Key}").ToList())}";

           
            var result = await Db.SqlManager().QueryAsync($"SELECT * FROM {nameof(AUDIT_BASE)} A1 " +
                $" JOIN {nameof(AUDIT_QUERY)} A2 ON A2.AB_ROWID = A1.AB_ROWID {whereStatement}", dictionary);

            var res = result.Adapt<List<AUDIT_QUERY>>();

            return res;
        }).OnComplete((TaskResult taskResult) =>
        {
            if (taskResult == TaskResult.Success)
            {
                AuditManager.AuditQueries(Context, parameters.UnPickleFromBytes<KeyValuePair<string, object>[]>());
                AuditManager.SaveChanges();
            }
        }); ;
    }
}