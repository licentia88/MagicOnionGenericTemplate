using MagicOnion;
using MagicT.Server.Database;
using MagicT.Shared.Services.Base;

namespace MagicT.Server.Services.Base;

/// <summary>
///     Base class for magic operations that involve a generic service, model, and database context.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public abstract class MagicServerService<TService, TModel> : DatabaseService<TService,TModel,MagicTContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
{

    public MagicServerService(IServiceProvider provider) : base(provider)
    {

    }
}