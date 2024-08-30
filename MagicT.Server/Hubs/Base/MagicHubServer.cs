using Benutomo;
using MagicOnion;
using MagicT.Shared.Hubs.Base;

namespace MagicT.Server.Hubs.Base;

/// <summary>
///     Base class for MagicOnion streaming hubs with CRUD operations, authorization, and database context capabilities.
///     This class is for structural purposes.
/// </summary>
/// <typeparam name="THub">The type of the streaming hub.</typeparam>
/// <typeparam name="TReceiver">The type of the receiver.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
[AutomaticDisposeImpl]
// ReSharper disable once UnusedType.Global
public abstract partial class MagicHubServer<THub, TReceiver, TModel, TContext> : MagicHubDataBase<THub, TReceiver, TModel, TContext>
    where THub : IStreamingHub<THub, TReceiver>
    where TReceiver : IMagicReceiver<TModel>
    where TContext : DbContext
    where TModel : class, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicHubServer{THub, TReceiver, TModel, TContext}"/> class.
    /// </summary>
    /// <param name="provider">The service provider for dependency resolution.</param>
    protected MagicHubServer(IServiceProvider provider) : base(provider)
    {
    }
}