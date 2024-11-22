using System.Linq.Expressions;
using Benutomo;
using Cysharp.Runtime.Multicast;
using EntityFramework.Exceptions.Common;
using Grpc.Core;
using MagicOnion;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Shared.Extensions;
using MagicT.Shared.Hubs.Base;
using MagicT.Shared.Models;
using Mapster;
using MessagePack;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Storage;

namespace MagicT.Server.Hubs.Base;

/// <summary>
/// A base class for MagicOnion streaming hubs with CRUD operations, authorization, and database context capabilities.
/// </summary>
/// <typeparam name="THub">The type of the streaming hub.</typeparam>
/// <typeparam name="TReceiver">The type of the receiver.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
[AutomaticDisposeImpl]
public abstract partial class MagicHubDataBase<THub, TReceiver, TModel, TContext> : MagicHubBase<THub, TReceiver,TModel>,
    IMagicHub<THub, TReceiver, TModel>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TContext : DbContext
    where TModel : class, new()
{
    [EnableAutomaticDispose]
    protected IDbContextTransaction Transaction;

    [EnableAutomaticDispose]
    protected TContext Db;
 
  
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicHubDataBase{THub,TReceiver,TModel,TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency resolution.</param>
    protected MagicHubDataBase(IServiceProvider provider) : base(provider)
    {
        Db = provider.GetService<TContext>();
        TokenManager = provider.GetService<TokenManager>();
     }

    /// <summary>
    /// Gets or sets the instance of <see cref="TokenManager"/>.
    /// </summary>
    [Inject]
    [EnableAutomaticDispose]
    public TokenManager TokenManager { get; set; }

    /// <summary>
    /// Connects the client asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing the connection ID.</returns>
    public virtual async Task<Guid> ConnectAsync()
    {
        Collection = new List<TModel>();
        
        Room = await Group.AddAsync(typeof(TModel).Name);
        
        // Storage = Room.GetInMemoryStorage<TModel>();
        
        return ConnectionId;
    }

    /// <summary>
    /// Creates a new model asynchronously.
    /// </summary>
    /// <param name="model">The model to create.</param>
    /// <returns>A task that represents the asynchronous operation, containing the created model.</returns>
    public virtual async Task<TModel> CreateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            await Db.Set<TModel>().AddAsync(model);
            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Add(model);
            Room.Except(ConnectionId).OnCreate(model);
            return model;
        });
    }

    /// <summary>
    /// Deletes a model asynchronously.
    /// </summary>
    /// <param name="model">The model to delete.</param>
    /// <returns>A task that represents the asynchronous operation, containing the deleted model.</returns>
    public virtual async Task<TModel> DeleteAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            Db.Set<TModel>().Remove(model);
            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Remove(model);
            Room.Except(ConnectionId).OnDelete(model);
            return model;
        });
    }

    /// <summary>
    /// Finds a list of entities of type TModel that are associated with a parent entity based on a foreign key.
    /// </summary>
    /// <param name="parentId">The identifier of the parent entity.</param>
    /// <param name="foreignKey">The foreign key used to associate the entities with the parent entity.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of entities.</returns>
    public virtual Task<List<TModel>> FindByParentAsync(string parentId, string foreignKey)
    {
        return ExecuteAsync(async () =>
        {
            KeyValuePair<string, object> parameter = new(foreignKey, parentId);
            var queryData = Db.BuildQuery<TModel>(parameter);
            var queryResult = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);
            return queryResult.Adapt<List<TModel>>();
        });
    }

    /// <summary>
    /// Finds a list of entities of type TModel based on the provided parameters.
    /// </summary>
    /// <param name="parameters">The parameters used to filter the entities.</param>
    /// <returns>A task that represents the asynchronous operation, containing a list of entities.</returns>
    public virtual Task<List<TModel>> FindByParametersAsync(byte[] parameters)
    {
        return ExecuteAsync(async () =>
        {
            var loParameters = parameters.DeserializeFromBytes<KeyValuePair<string, Object>[]>();
            
            var queryData = Db.BuildQuery<TModel>(loParameters);
            
            var result = await Db.SqlManager().QueryAsync(queryData.query, queryData.parameters);
            
            return result.Adapt<List<TModel>>();
        });
    }

    /// <summary>
    /// Reads the data asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task ReadAsync()
    {
        await ExecuteAsync(async () =>
        {
            var data = await Db.Set<TModel>().Where(x => !Collection.Contains(x)).AsNoTracking().ToListAsync();
            Room.Single(ConnectionId).OnRead(data);
            Collection.AddRange(data);
            return Collection;
        });
    }

    /// <summary>
    /// Streams the data asynchronously in batches.
    /// </summary>
    /// <param name="batchSize">The size of each batch to fetch from the database.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task StreamReadAsync(int batchSize)
    {
        await foreach (var data in FetchStreamAsync(batchSize))
        {
            Room.Single(ConnectionId).OnStreamRead(data);
            Collection.AddRange(data);
        }
    }

    /// <summary>
    /// Updates a model asynchronously.
    /// </summary>
    /// <param name="model">The model to update.</param>
    /// <returns>A task that represents the asynchronous operation, containing the updated model.</returns>
    public virtual async Task<TModel> UpdateAsync(TModel model)
    {
        return await ExecuteAsync(async () =>
        {
            var existing = Db.Entry(model).OriginalValues.ToModel<TModel>();
            Db.Set<TModel>().Attach(model);
            Db.Set<TModel>().Update(model);
            await Db.SaveChangesAsync();
            Db.ChangeTracker.Clear();
            Collection.Replace(existing, model);
            Room.Except(ConnectionId).OnUpdate(model);
            return model;
        });
    }

    /// <summary>
    /// Updates the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task CollectionChanged()
    {
        var newCollection = await Db.Set<TModel>().AsNoTracking().ToListAsync();
        Collection.Clear();
        Collection.AddRange(newCollection);
        Room.Single(ConnectionId).OnCollectionChanged(Collection);
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
    /// <returns>A task that represents the asynchronous operation, containing the result.</returns>
    protected virtual async Task<T> ExecuteAsync<T>(Func<Task<T>> task)
    {
        try
        {
            var result = await task();
            if (Transaction is not null)
                await Transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            if (Transaction is not null)
                await Transaction.RollbackAsync();
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
    }

    /// <summary>
    /// Executes a task and handles transactions.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="task">The task to execute.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result.</returns>
    public virtual Task Execute<T>(Func<T> task)
    {
        try
        {
            var result = task();
            if (Transaction is not null)
                Transaction.Commit();
            return Task.FromResult(result);
        }
        catch (UniqueConstraintException ex)
        {
            Console.WriteLine("A unique constraint violation occurred: " + ex.Message);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
        catch (ReferenceConstraintException ex)
        {
            Console.WriteLine("A reference constraint violation occurred: " + ex.Message);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
        catch (CannotInsertNullException ex)
        {
            Console.WriteLine("A not null constraint violation occurred: " + ex.Message);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
        catch (MaxLengthExceededException ex)
        {
            Console.WriteLine("A max length constraint violation occurred: " + ex.Message);
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
        catch (Exception)
        {
            if (Transaction is not null)
                Transaction.Rollback();
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
    }

    /// <summary>
    /// Executes an action and handles transactions.
    /// </summary>
    /// <param name="task">The action to execute.</param>
    public virtual void Execute(Action task)
    {
        try
        {
            task();
            if (Transaction is not null)
                Transaction.Commit();
        }
        catch (Exception)
        {
            if (Transaction is not null)
                Transaction.Rollback();
            throw new ReturnStatusException(StatusCode.Cancelled, "Error Description");
        }
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

    /// <summary>
    /// Keeps the connection alive asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task KeepAliveAsync()
    {
        return Task.CompletedTask;
    }
}