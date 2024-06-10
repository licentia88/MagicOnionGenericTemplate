using Benutomo;
using MagicT.Server.Extensions;
using MagicOnion;
using MagicT.Shared.Enums;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace MagicT.Server.Services.Base;

[AutomaticDisposeImpl]
// ReSharper disable once RedundantExtendsListEntry
// ReSharper disable once RedundantExtendsListEntry
// ReSharper disable once RedundantExtendsListEntry
public abstract partial class AuditDatabaseService<TService, TModel, TContext> : MagicServerService<TService, TModel, TContext>, IDisposable,IAsyncDisposable
    where TContext : DbContext
    where TModel : class
    where TService : IMagicService<TService, TModel>, IService<TService>
{
    protected AuditDatabaseService(IServiceProvider provider) : base(provider) {  }

    public override UnaryResult<TModel> CreateAsync(TModel model)
    {
        return ExecuteAsync(async () =>
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
        return base.ReadAsync().OnComplete(taskResult =>
        {
            if (taskResult != TaskResult.Success) return;
            AuditManager.AuditQueries(Context, string.Empty);
            AuditManager.SaveChanges();
        });
    }

    public override UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return ExecuteAsync(async () =>
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
        return ExecuteAsync(async () =>
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
        return base.FindByParentAsync(parentId, foreignKey).OnComplete(taskResult =>
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
        return base.FindByParametersAsync(parameters).OnComplete(taskResult =>
        {
            if (taskResult == TaskResult.Success)
            {
                AuditManager.AuditQueries(Context, parameters);
                AuditManager.SaveChanges();
            }
        });
    }

    protected override UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        return base.ExecuteAsync(task, callerFilePath, callerMemberName,callerLineNumber).OnComplete((model, taskResult, exception) =>
        {
            if (taskResult != TaskResult.Fail) return;
            AuditManager.AuditFailed(Context, exception.Message, model);
            AuditManager.SaveChanges();
        });
    }


    protected override UnaryResult<T> Execute<T>(Func<T> task, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        return base.Execute(task,callerFilePath,callerMemberName,callerLineNumber).OnComplete((model, taskResult, exception) =>
        {
            if (taskResult == TaskResult.Fail)
            {
                AuditManager.AuditFailed(Context, exception.Message, model);
                AuditManager.SaveChanges();
            }
        });
    }
}
