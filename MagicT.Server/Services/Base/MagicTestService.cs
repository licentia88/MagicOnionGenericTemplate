using MagicOnion;
using MagicT.Shared.Services.Base;

namespace MagicT.Server.Services.Base;

/// <summary>
///     A specialized service class intended for unit testing magic operations
///     involving a generic service, model, and database context.
///     This class inherits from the base <see cref="DatabaseService{TService, TModel, TContext}"/> 
///     to provide the same functionality in a testing environment, facilitating isolated 
///     and focused unit tests on service-layer logic without needing to rely on full application 
///     infrastructure.
/// </summary>
/// <typeparam name="TService">The type of the service being tested.</typeparam>
/// <typeparam name="TModel">The type of the model associated with the service.</typeparam>
/// <typeparam name="TContext">The type of the DbContext, used to mock or simulate database interactions.</typeparam>
public class MagicTestService<TService, TModel, TContext> : DatabaseService<TService, TModel, TContext>
    where TService : IMagicService<TService, TModel>, IService<TService>
    where TModel : class
    where TContext : DbContext
{
    
   
    // ReSharper disable once PublicConstructorInAbstractClass
    /// <summary>
    ///     Initializes a new instance of the <see cref="MagicTestService{TService, TModel, TContext}"/> class.
    ///     The constructor takes an <see cref="IServiceProvider"/> to facilitate dependency injection,
    ///     allowing for the configuration of mocked services or in-memory contexts for testing purposes.
    /// </summary>
    /// <param name="provider">
    ///     The service provider used to resolve dependencies, including mock implementations for unit testing.
    /// </param>
    public MagicTestService(IServiceProvider provider) : base(provider)
    {
    }
    
    ~MagicTestService()
    {
        Dispose(false);
    }
}