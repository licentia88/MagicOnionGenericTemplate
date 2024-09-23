using Benutomo;
using Cysharp.Runtime.Multicast;
using MagicOnion;
using MagicT.Server.Extensions;
using MagicT.Server.Helpers;
using MagicT.Server.Managers;
using MagicT.Shared.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs.Base;
using Mapster;

namespace MagicT.Server.Hubs.Base;

/// <summary>
/// A base class for MagicOnion streaming hubs with CRUD operations, authorization, database context capabilities, and audit functionality.
/// </summary>
/// <typeparam name="THub">The type of the streaming hub.</typeparam>
/// <typeparam name="TReceiver">The type of the receiver.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
[AutomaticDisposeImpl]
// ReSharper disable once UnusedType.Global
public abstract partial class MagicHubAuditServer<THub, TReceiver, TModel, TContext> : MagicHubDataBase<THub, TReceiver, TModel, TContext>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TContext : DbContext
    where TModel : class, new()
{
    /// <summary>
    /// Gets or sets the AuditManager instance for auditing operations.
    /// This property is automatically disposed when the class is disposed.
    /// </summary>
    [EnableAutomaticDispose]
    protected AuditManager AuditManager { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicHubAuditServer{THub, TReceiver, TModel, TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency resolution.</param>
    protected MagicHubAuditServer(IServiceProvider provider) : base(provider)
    {
        AuditManager = provider.GetRequiredService<AuditManager>();
    }

    /// <summary>
    /// Creates a new model asynchronously.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>The created model.</returns>
    public override async Task<TModel> CreateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            await Db.Set<TModel>().AddAsync(model);
            AuditManager.AuditRecords(Context, Db.ChangeTracker.Entries());
            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Add(model);
            Room.Except(ConnectionId).OnCreate(model);
            // Room.Except(ConnectionId).OnCreate(model);
            AuditManager.SaveChanges();
            return model;
        });
    }

    /// <summary>
    /// Deletes a model asynchronously.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>The deleted model.</returns>
    public override async Task<TModel> DeleteAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);
            AuditManager.AuditRecords(Context, Db.ChangeTracker.Entries());
            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Remove(model);
            Room.Except(ConnectionId).OnDelete(model);
            AuditManager.SaveChanges();
            return model;
        });
    }

    /// <summary>
    /// Finds models by parent ID and foreign key asynchronously.
    /// </summary>
    /// <param name="parentId">The parent ID.</param>
    /// <param name="foreignKey">The foreign key.</param>
    /// <returns>A list of models.</returns>
    public override async Task<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return await ExecuteAsync(async () =>
        {
            KeyValuePair<string, object> parameter = new(foreignKey, parentId);
            var queryData = QueryBuilder.BuildQuery<TModel>(parameter);
            var queryResult = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);
            AuditManager.AuditQueries(Context, parentId, foreignKey);
            AuditManager.SaveChanges();
            return queryResult.Adapt<List<TModel>>();
        });
    }

    /// <summary>
    /// Finds models by parameters asynchronously.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A list of models.</returns>
    public override async Task<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
    
        return await base.FindByParametersAsync(parameters).OnComplete((_, taskResult, _) =>
        {
            if (taskResult != TaskResult.Success) return;
            AuditManager.AuditQueries(Context, string.Empty);
            AuditManager.SaveChanges();
        });
    }

    /// <summary>
    /// Reads models asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task ReadAsync()
    {
        await base.ReadAsync().OnComplete(taskResult =>
        {
            if (taskResult != TaskResult.Success) return;
            AuditManager.AuditQueries(Context, string.Empty);
            AuditManager.SaveChanges();
        });
    }

    /// <summary>
    /// Updates a model asynchronously.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>The updated model.</returns>
    public override async Task<TModel> UpdateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            var existing = Db.Entry(model).OriginalValues.ToModel<TModel>();
            Db.Set<TModel>().Attach(model);
            Db.Set<TModel>().Update(model);
            AuditManager.AuditRecords(Context, Db.ChangeTracker.Entries());
            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Replace(existing, model);
            Room.Except(ConnectionId).OnUpdate(model);
            AuditManager.SaveChanges();
            return model;
        });
    }
}