using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Benutomo;
using MagicOnion;
using MagicT.Server.Extensions;
using MagicT.Server.Managers;
using MagicT.Shared.Enums;
using MagicT.Shared.Services.Base;
using Mapster;
using Microsoft.EntityFrameworkCore.Storage;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
// ReSharper disable ExplicitCallerInfoArgument

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
    [EnableAutomaticDispose]
    protected TContext Db { get; set; }

    [EnableAutomaticDispose]
    protected AuditManager AuditManager { get; set; }

    [EnableAutomaticDispose]
    protected QueryManager QueryManager { get; set; }

    [EnableAutomaticDispose]
    protected IDbContextTransaction Transaction;

    protected DatabaseService(IServiceProvider provider) : base(provider)
    {
        Db = provider.GetService<TContext>();
        AuditManager = provider.GetService<AuditManager>();
        QueryManager = provider.GetService<QueryManager>();
    }

    public virtual async UnaryResult<TModel> CreateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Add(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    public async UnaryResult<List<TModel>> CreateRangeAsync(List<TModel> models)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().AddRange(models);
            await Db.SaveChangesAsync();
            return models;
        });
    }

    public virtual async UnaryResult<List<TModel>> ReadAsync()
    {
        return await ExecuteAsync(async () => await Db.Set<TModel>().ToListAsync());
    }

    public virtual async UnaryResult<TModel> UpdateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    public async UnaryResult<List<TModel>> UpdateRangeAsync(List<TModel> models)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().UpdateRange(models);
            await Db.SaveChangesAsync();
            return models;
        });
    }

    public virtual async UnaryResult<TModel> DeleteAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);
            await Db.SaveChangesAsync();
            return model;
        });
    }

    public async UnaryResult<List<TModel>> DeleteRangeAsync(List<TModel> models)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().RemoveRange(models);
            await Db.SaveChangesAsync();
            return models;
        });
    }

    public virtual async UnaryResult<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object> parameter = new(foreignKey, parentId);
            var queryData = QueryManager.BuildQuery<TModel>(parameter);
            var queryResult = await Db.Manager().QueryAsync(queryData.query, queryData.parameters);
            return queryResult.Adapt<List<TModel>>();
        });
    }

    public virtual async UnaryResult<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return await ExecuteAsync(async () =>
        {
            var queryData = QueryManager.BuildQuery<TModel>(parameters);
            var result = await Db.Manager().QueryAsync(queryData.query, queryData.parameters);
            return result.Adapt<List<TModel>>();
        });
    }

    public virtual async Task<ServerStreamingResult<List<TModel>>> StreamReadAllAsync(int batchSize)
    {
        var stream = GetServerStreamingContext<List<TModel>>();
        await foreach (var data in FetchStreamAsync(batchSize))
            await stream.WriteAsync(data);
        return stream.Result();
    }

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

    protected override async UnaryResult<T> ExecuteAsync<T>(Func<Task<T>> task, string callerFilePath = default, string callerMemberName = default,
        int callerLineNumber = default)
    {
        return await base.ExecuteAsync(task, callerFilePath, callerMemberName, callerLineNumber).OnComplete((_, result, _) =>
        {
            if (result == TaskResult.Success)
                Execute(() => Transaction?.Commit(), callerFilePath, callerMemberName, callerLineNumber, "Commit Transaction");
            else
                Execute(() => Transaction?.Rollback(), GetType().Name, callerMemberName, callerLineNumber, "Rollback Transaction");
        });
    }

    protected override async UnaryResult<T> ExecuteAsync<T>(Func<T> task, string callerFilePath = default, string callerMemberName = default,
        int callerLineNumber = default)
    {
        return await base.ExecuteAsync(task, callerFilePath, callerMemberName, callerLineNumber).OnComplete((_, result, _) =>
        {
            if (result == TaskResult.Success)
                Execute(() => Transaction?.Commit(), callerFilePath, callerMemberName, callerLineNumber, "Commit Transaction");
            else
                Execute(() => Transaction?.Rollback(), GetType().Name, callerMemberName, callerLineNumber, "Rollback Transaction");
        });
    }

    protected override async Task ExecuteAsync(Func<Task> task, string callerFilePath = default, string callerMemberName = default,
        int callerLineNumber = default)
    {
        await base.ExecuteAsync(task, callerFilePath, callerMemberName, callerLineNumber).OnComplete(result =>
        {
            if (result == TaskResult.Success)
                Execute(() => Transaction?.Commit(), callerFilePath, callerMemberName, callerLineNumber, "Commit Transaction");
            else
                Execute(() => Transaction?.Rollback(), callerFilePath, callerMemberName, callerLineNumber, "Rollback Transaction");
        });
    }

    protected override void HandleError(Exception ex, string callerFilePath, string callerMemberName, int callerLineNumber)
    {
        Transaction?.Rollback();
        base.HandleError(ex, callerFilePath, callerMemberName, callerLineNumber);
    }

    protected void BeginTransaction()
    {
        Transaction = Db.Database.BeginTransaction();
    }

    protected async Task BeginTransactionAsync()
    {
        Transaction = await Db.Database.BeginTransactionAsync();
    }
}