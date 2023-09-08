using AQueryMaker;
using MagicOnion;
using MagicOnion.Server;
using MagicT.Server.Database;
using MagicT.Server.Exceptions;
using MagicT.Server.HostedServices;
using MagicT.Server.ZoneTree;
using MagicT.Shared.Services.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services.Base;

/// <summary>
/// Base class for magic operations that involve a generic service, model, and database context.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
public abstract partial class MagicServerServiceBase<TService, TModel> : MagicServerServiceBase<TService, TModel, MagicTContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicServerServiceBase{TService,TModel}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public MagicServerServiceBase(IServiceProvider provider) : base(provider)
    {
        // This constructor initializes the base class with the provided service provider.
        // It allows you to perform operations involving the generic service, model, and database context.
    }
}

 
/// <summary>
///     Base class for magic operations that involve a generic service, model, and database context.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public partial class MagicServerServiceBase<TService, TModel, TContext> : ServiceBase<TService>, IMagicService<TService, TModel>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{


    /// <summary>
    /// Initializes a new instance of the <see cref="MagicServerServiceBase{TService,TModel,TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    // ReSharper disable once MemberCanBeProtected.Global
    public MagicServerServiceBase(IServiceProvider provider)
    {
        ZoneDbManager = provider.GetService<ZoneDbManager>();
        BackGroundTaskQueue = provider.GetService<IBackGroundTaskQueue>();

        Logger = provider.GetService<ILogger<TModel>>();
        // Initialize the MemoryDatabase property with the instance retrieved from the service provider.
        //MemoryDatabaseManager = provider.GetService<MemoryDatabaseManager>();

        DbExceptionHandler = provider.GetService<DbExceptionHandler>();

        // Initialize the Db field with the instance of the database context retrieved from the service provider.
        Db = provider.GetService<TContext>();

        // Initialize the ConnectionFactory field with the instance of the dictionary retrieved from the service provider.
        ConnectionFactory = provider.GetService<IDictionary<string, Func<SqlQueryFactory>>>();    
    }

    
}