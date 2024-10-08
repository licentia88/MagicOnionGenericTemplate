﻿using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.Server.Services.Base;

/// <summary>
///     Base class for magic operations that involve a generic service, model, and database context.
///     
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of DbContext</typeparam>
public  abstract class MagicServerService<TService, TModel,TContext> : DatabaseService<TService,TModel, TContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext: DbContext
{

    // ReSharper disable once PublicConstructorInAbstractClass
    protected MagicServerService(IServiceProvider provider) : base(provider)
    {
         
    }
}