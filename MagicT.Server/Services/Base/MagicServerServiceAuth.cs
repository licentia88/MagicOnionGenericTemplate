using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Filters;
using MagicT.Server.Invocables;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

[MagicTAuthorize]

public abstract class MagicServerServiceAuth<TService, TModel, TContext> : DatabaseService<TService, TModel, MagicTContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : MagicTContext
{

    public MagicServerServiceAuth(IServiceProvider provider) : base(provider)
    {
    }


    private void AuditRecords() => Database.ChangeTracker.Entries().ToList()
                                  .ForEach(x => Queue.QueueInvocableWithPayload<AuditRecordsInvocable<MagicTContext>, AuditRecordPayload>(new AuditRecordPayload(x, Token(Context).Id, Context.ServiceType.Name, Context.MethodInfo.Name, Context.CallContext.Method)));


    public override UnaryResult<TModel> CreateAsync(TModel model)
    {
        Logs = AuditRecords;

        return base.CreateAsync(model);
        //return ExecuteWithoutResponseAsync(async () =>
        //{
        //    Database.Set<TModel>().Add(model);

        //    AuditRecords();

        //    await Database.SaveChangesAsync();

        //    return model;
        //}, Transaction);
    }

    //TODO:Read yapma logu ekle
    public override UnaryResult<List<TModel>> ReadAsync()
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            AuditQueries();

            return await Database.Set<TModel>().AsNoTracking().ToListAsync();
        });

    }

    public override UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            Database.Set<TModel>().Update(model);

            AuditRecords();

            await Database.SaveChangesAsync();

            return model;

        }, Transaction);
    }

    public override UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            Database.Set<TModel>().Remove(model);

            AuditRecords();

            await Database.SaveChangesAsync();

            return model;
        }, Transaction);
    }


    public override UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
         return base.FindByParametersAsync(parameters);
    }

    public override UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
         return base.FindByParentAsync(parentId, foreignKey);
    }

}
