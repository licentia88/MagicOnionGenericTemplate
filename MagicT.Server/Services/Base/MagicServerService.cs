using Benutomo;
using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.Server.Services.Base;

/// <summary>
///     Base class for magic operations that involve a generic service, model, and database context.
///     
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of DbContext</typeparam>
[AutomaticDisposeImpl]
public  abstract partial class MagicServerService<TService, TModel, TContext>(IServiceProvider provider) : DatabaseService<TService, TModel, TContext>(provider)
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{
    
    ~MagicServerService()
    {
        Dispose(false);
    }
}