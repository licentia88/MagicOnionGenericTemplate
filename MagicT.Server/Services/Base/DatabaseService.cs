using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Benutomo;
using MagicOnion;
using MagicT.Server.Extensions;
using MagicT.Server.Managers;
using MagicT.Shared.Enums;
using MagicT.Shared.Services.Base;
using Mapster;
using Microsoft.EntityFrameworkCore.Storage;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
using QueryBuilder = MagicT.Server.Helpers.QueryBuilder;


namespace MagicT.Server.Services.Base;

/// <summary>
/// Base class for database services providing common database operations.
/// </summary>
/// <typeparam name="TService">The service interface.</typeparam>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TContext">The database context type.</typeparam>
[AutomaticDisposeImpl]
public abstract partial class DatabaseService<TService, TModel, TContext> : MagicServerBase<TService>, IMagicService<TService, TModel>
    where TContext : DbContext
    where TModel : class
    where TService : IMagicService<TService, TModel>, IService<TService>
{
    /// <summary>
    /// The database context.
    /// </summary>
    [EnableAutomaticDispose]
    protected TContext Db { get; set; }

    /// <summary>
    /// The audit manager.
    /// </summary>
    [EnableAutomaticDispose]
    protected AuditManager AuditManager { get; set; }
 
    /// <summary>
    /// The database transaction.
    /// </summary>
    [EnableAutomaticDispose]
    protected IDbContextTransaction Transaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService{TService, TModel, TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    protected DatabaseService(IServiceProvider provider) : base(provider)
    {
        Db = provider.GetService<TContext>();
        AuditManager = provider.GetService<AuditManager>();
     }

    /// <summary>
    /// Creates a new model asynchronously.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>The created model.</returns>
    public virtual async UnaryResult<TModel> CreateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Add(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    /// <summary>
    /// Creates a range of models asynchronously.
    /// </summary>
    /// <param name="models">The models to create.</param>
    /// <returns>The created models.</returns>
    public async UnaryResult<List<TModel>> CreateRangeAsync(List<TModel> models)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().AddRange(models);
            await Db.SaveChangesAsync();
            return models;
        });
    }

    /// <summary>
    /// Reads all models asynchronously.
    /// </summary>
    /// <returns>The list of models.</returns>
    public virtual async UnaryResult<List<TModel>> ReadAsync()
    {
        return await ExecuteAsync(async () => await Db.Set<TModel>().ToListAsync());
    }

    /// <summary>
    /// Updates a model asynchronously.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>The updated model.</returns>
    public virtual async UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    /// <summary>
    /// Updates a range of models asynchronously.
    /// </summary>
    /// <param name="models">The models to update.</param>
    /// <returns>The updated models.</returns>
    public async UnaryResult<List<TModel>> UpdateRangeAsync(List<TModel> models)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().UpdateRange(models);
            await Db.SaveChangesAsync();
            return models;
        });
    }

    /// <summary>
    /// Deletes a model asynchronously.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>The deleted model.</returns>
    public virtual async UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    /// <summary>
    /// Deletes a range of models asynchronously.
    /// </summary>
    /// <param name="models">The models to delete.</param>
    /// <returns>The deleted models.</returns>
    public async UnaryResult<List<TModel>> DeleteRangeAsync(List<TModel> models)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().RemoveRange(models);
            await Db.SaveChangesAsync();
            return models;
        });
    }

    /// <summary>
    /// Finds models by parent ID and foreign key asynchronously.
    /// </summary>
    /// <param name="parentId">The parent ID.</param>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>The list of models.</returns>
    public virtual async UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object> parameter = new(foreignKey, parentId);
            var queryData = QueryBuilder.BuildQuery<TModel>(parameter);
            var queryResult = await Db.Manager().QueryAsync(queryData.query, queryData.parameters);
            return queryResult.Adapt<List<TModel>>();
        });
    }

    /// <summary>
    /// Finds models by parameters asynchronously.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The list of models.</returns>
    public virtual async UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return await ExecuteAsync(async () =>
        {
            var queryData = QueryBuilder.BuildQuery<TModel>(parameters);
            var result = await Db.Manager().QueryAsync(queryData.query, queryData.parameters);
            return result.Adapt<List<TModel>>();
        });
    }

    /// <summary>
    /// Streams all models asynchronously in batches.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>The server streaming result containing the list of models.</returns>
    public virtual async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        var stream = GetServerStreamingContext<List<TModel>>();
        await foreach (var data in FetchStreamAsync(batchSize))
            await stream.WriteAsync(data);
        return stream.Result();
    }

    /// <summary>
    /// Fetches models asynchronously in batches.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>An asynchronous enumerable that yields lists of models.</returns>
    protected async IAsyncEnumerable<List<TModel>> FetchStreamAsync(int batchSize = 10)
    {
        var modelType = typeof(TModel);
        var keyProperty = modelType.GetProperties()
            .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Any());

        if (keyProperty == null)
        {
            throw new InvalidOperationException("No key property found on model.");
        }

        var count = await Db.Set<TModel>().CountAsync();
        var batches = (int)Math.Ceiling((double)count / batchSize);
        var lastFetchedKey = default(object);

        for (var i = 0; i < batches; i++)
        {
            var query = Db.Set<TModel>().AsNoTracking();

            if (lastFetchedKey != null)
            {
                var parameter = Expression.Parameter(modelType, "e");
                var property = Expression.Property(parameter, keyProperty.Name);
                var constant = Expression.Constant(lastFetchedKey);
                var comparison = Expression.GreaterThan(property, constant);
                var lambda = Expression.Lambda<Func<TModel, bool>>(comparison, parameter);

                query = query.Where(lambda);
            }

            var entities = await query.Take(batchSize).ToListAsync();

            if (!entities.Any())
            {
                break;
            }

            lastFetchedKey = keyProperty.GetValue(entities.Last());
            yield return entities;
        }
    }

    /// <summary>
    /// Executes an asynchronous task and handles transactions.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The asynchronous task to execute.</param>
    /// <param name="callerFilePath">The caller file path.</param>
    /// <param name="callerMemberName">The caller member name.</param>
    /// <param name="callerLineNumber">The caller line number.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result.</returns>
    protected override async UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task,[CallerFilePath] string callerFilePath = default, [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        return await base.ExecuteAsync(task, callerMemberName:callerMemberName).OnComplete((_, result, _) =>
        {
            if (result == TaskResult.Success)
                Execute(() => Transaction?.Commit(), callerMemberName:callerMemberName, message:"Commit Transaction");
            else
                Execute(() => Transaction?.Rollback(), callerMemberName:callerMemberName, message:"Rollback Transaction");
        });
    }

    /// <summary>
    /// Executes a task and handles transactions.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The task to execute.</param>
    /// <param name="callerFilePath">The caller file path.</param>
    /// <param name="callerMemberName">The caller member name.</param>
    /// <param name="callerLineNumber">The caller line number.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result.</returns>
    protected override async UnaryResult<T> ExecuteAsync<T>(Func<T> task,[CallerFilePath] string callerFilePath = default,[CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        return await base.ExecuteAsync(task,callerMemberName: callerMemberName).OnComplete((_, result, _) =>
        {
            if (result == TaskResult.Success)
                Execute(() => Transaction?.Commit(), callerMemberName:callerMemberName, message:"Commit Transaction");
            else
                Execute(() => Transaction?.Rollback(),callerMemberName: callerMemberName,message: "Rollback Transaction");
        });
    }

    /// <summary>
    /// Executes an asynchronous action and handles transactions.
    /// </summary>
    /// <param name="task">The asynchronous action to execute.</param>
    /// <param name="callerFilePath">The caller file path.</param>
    /// <param name="callerMemberName">The caller member name.</param>
    /// <param name="callerLineNumber">The caller line number.</param>
    protected override async Task ExecuteAsync(Func<Task> task, [CallerFilePath] string callerFilePath = default, [CallerMemberName] string callerMemberName = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        await base.ExecuteAsync(task,callerMemberName:callerMemberName).OnComplete(result =>
        {
            if (result == TaskResult.Success)
                Execute(() => Transaction?.Commit(), callerMemberName: callerMemberName,message:"Commit Transaction");
            else
                Execute(() => Transaction?.Rollback(), callerMemberName: callerMemberName,message: "Rollback Transaction");
        });
    }

    /// <summary>
    /// Handles errors by rolling back the transaction and calling the base error handler.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="callerFilePath">The caller file path.</param>
    /// <param name="callerMemberName">The caller member name.</param>
    /// <param name="callerLineNumber">The caller line number.</param>
    protected override void HandleError(Exception ex, string callerFilePath, string callerMemberName, int callerLineNumber)
    {
        Transaction?.Rollback();
        base.HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Begins a transaction.
    /// </summary>
    protected void BeginTransaction()
    {
        Transaction = Db.Database.BeginTransaction();
    }

    /// <summary>
    /// Begins a transaction asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task BeginTransactionAsync()
    {
        Transaction = await Db.Database.BeginTransactionAsync();
    }
}