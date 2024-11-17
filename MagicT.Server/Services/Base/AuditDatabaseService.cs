using Benutomo;
using MagicT.Server.Extensions;
using MagicOnion;
using MagicT.Shared.Enums;
using MagicT.Shared.Services.Base;
using System.Runtime.CompilerServices;


namespace MagicT.Server.Services.Base;

/// <summary>
/// A base service class that provides audit functionality for database operations.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
[AutomaticDisposeImpl]
public abstract partial class AuditDatabaseService<TService, TModel, TContext> : MagicServerService<TService, TModel, TContext>
    where TContext : DbContext
    where TModel : class
    where TService : IMagicService<TService, TModel>, IService<TService>
{
    
    ~AuditDatabaseService()
    {
        Dispose();
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditDatabaseService{TService,TModel,TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    protected AuditDatabaseService(IServiceProvider provider) : base(provider) { }

    /// <summary>
    /// Creates a new model asynchronously and audits the operation.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>The created model.</returns>
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

    /// <summary>
    /// Reads all models asynchronously and audits the operation.
    /// </summary>
    /// <returns>A list of models.</returns>
    public override UnaryResult<List<TModel>> ReadAsync()
    {
        return base.ReadAsync().OnComplete(taskResult =>
        {
            if (taskResult != TaskResult.Success) return;
            AuditManager.AuditQueries(Context, string.Empty);
            AuditManager.SaveChanges();
        });
    }

    /// <summary>
    /// Updates an existing model asynchronously and audits the operation.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>The updated model.</returns>
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

    /// <summary>
    /// Deletes an existing model asynchronously and audits the operation.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>The deleted model.</returns>
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

    /// <summary>
    /// Finds models by parent ID and foreign key asynchronously and audits the operation.
    /// </summary>
    /// <param name="parentId">The parent ID.</param>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>A list of models.</returns>
    public override UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return base.FindByParentAsync(parentId, foreignKey).OnComplete(taskResult =>
        {
            if (taskResult != TaskResult.Success) return;
            AuditManager.AuditQueries(Context, parentId, foreignKey);
            AuditManager.SaveChanges();
        });
    }

    /// <summary>
    /// Finds models by parameters asynchronously and audits the operation.
    /// </summary>
    /// <param name="parameters">The parameters to search by.</param>
    /// <returns>A list of models.</returns>
    public override UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return base.FindByParametersAsync(parameters).OnComplete(taskResult =>
        {
            if (taskResult != TaskResult.Success) return;
            AuditManager.AuditQueries(Context, parameters);
            AuditManager.SaveChanges();
        });
    }

    /// <summary>
    /// Executes an asynchronous task with audit functionality.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <returns>The result of the task.</returns>
    protected override UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task, [CallerFilePath] string callerFilePath = default, [CallerMemberName] string callerMemberName = default, [CallerLineNumber] int callerLineNumber = 0)
    {
        return base.ExecuteAsync(task, callerMemberName: callerMemberName).OnComplete((model, taskResult, exception) =>
        {
            if (taskResult != TaskResult.Fail) return;
            AuditManager.AuditFailed(Context, exception.Message, model);
            AuditManager.SaveChanges();
        });
    }

    /// <summary>
    /// Executes a synchronous task with audit functionality.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The task to execute.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The name of the caller member.</param>
    /// <param name="callerLineNumber">The line number in the source file at which the method is called.</param>
    /// <returns>The result of the task.</returns>
    protected override UnaryResult<T> ExecuteAsync<T>(Func<T> task, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        return base.ExecuteAsync(task, callerMemberName: callerMemberName).OnComplete((model, taskResult, exception) =>
        {
            if (taskResult != TaskResult.Fail) return;
            AuditManager.AuditFailed(Context, exception.Message, model);
            AuditManager.SaveChanges();
        });
    }
}