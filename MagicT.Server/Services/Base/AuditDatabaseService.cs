using MagicOnion;
using MagicT.Server.Extensions;
using MagicT.Server.Managers;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

public class AuditDatabaseService<TService, TModel, TContext> : DatabaseService<TService, TModel, TContext>
    where TContext : DbContext
    where TModel : class
    where TService : IMagicService<TService, TModel>, IService<TService>
{
    public AuditDatabaseService(IServiceProvider provider) : base(provider) {  }

    public override UnaryResult<TModel> CreateAsync(TModel model)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            Db.Set<TModel>().Add(model);

            AuditManager.AuditRecords(Context, Db.ChangeTracker.Entries());

            await Db.SaveChangesAsync();

            AuditManager.SaveChanges();

            return model;
        });

       
    }

    public override UnaryResult<List<TModel>> ReadAsync()
    {
        return base.ReadAsync().OnComplete((TaskResult taskResult) =>
        {
            if (taskResult == TaskResult.Success)
            {
                AuditManager.AuditQueries(Context, string.Empty);
                AuditManager.SaveChanges();
            }
        });
    }

    public override UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            Db.Set<TModel>().Update(model);

            AuditManager.AuditRecords(Context, Db.ChangeTracker.Entries());

            await Db.SaveChangesAsync();

            AuditManager.SaveChanges();

            return model;
        });
    }

    public override UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return ExecuteWithoutResponseAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);

            AuditManager.AuditRecords(Context, Db.ChangeTracker.Entries());

            await Db.SaveChangesAsync();

            AuditManager.SaveChanges();

            return model;
        });

       
    }

    public override UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return base.FindByParentAsync(parentId, foreignKey).OnComplete((TaskResult taskResult) =>
        {
            if (taskResult == TaskResult.Success)
            {
                AuditManager.AuditQueries(Context, parentId, foreignKey);
                AuditManager.SaveChanges();
            }
        });
    }

    public override UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return base.FindByParametersAsync(parameters).OnComplete((TaskResult taskResult) =>
        {
            if (taskResult == TaskResult.Success)
            {
                AuditManager.AuditQueries(Context, parameters);
                AuditManager.SaveChanges();
            }
        });
    }



    protected override UnaryResult<T> ExecuteWithoutResponseAsync<T>(Func<Task<T>> task)
    {
        return base.ExecuteWithoutResponseAsync(task).OnComplete((T model, TaskResult taskResult, Exception exception) =>
        {
            if (taskResult == TaskResult.Fail)
            {
                AuditManager.AuditFailed(Context, exception.Message, model);
                AuditManager.SaveChanges();
            }
        });
    }

    public override UnaryResult<T> ExecuteWithoutResponse<T>(Func<T> task)
    {
        return base.ExecuteWithoutResponse(task).OnComplete((T model, TaskResult taskResult, Exception exception) =>
        {
            if (taskResult == TaskResult.Fail)
            {
                AuditManager.AuditFailed(Context, exception.Message,model);
                AuditManager.SaveChanges();
            }
        });
    }
}
