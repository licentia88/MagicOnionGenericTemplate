using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Initializers;

/// <summary>
/// Initializes data for the application.
/// </summary>
[RegisterScoped]
public class DataInitializer
{
    /// <summary>
    /// Gets or sets the lazy-loaded collection of authorizations.
    /// </summary>
    public Lazy<List<AUTHORIZATIONS_BASE>> AUTHORIZATIONS_BASE { get; set; }

    /// <summary>
    /// Gets or sets the lazy-loaded collection of users.
    /// </summary>
    public Lazy<List<USERS>> USERS { get; set; }

    /// <summary>
    /// Gets or sets the lazy-loaded collection of operations.
    /// </summary>
    public Lazy<List<Operations>> Operations { get; set; }

    /// <summary>
    /// Gets or sets the service for initializing data.
    /// </summary>
    public IInitializerService InitializerService { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataInitializer"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public DataInitializer(IServiceProvider provider)
    {
        AUTHORIZATIONS_BASE = provider.GetService<Lazy<List<AUTHORIZATIONS_BASE>>>();
        USERS = provider.GetService<Lazy<List<USERS>>>();
        Operations = provider.GetService<Lazy<List<Operations>>>();
        InitializerService = provider.GetService<IInitializerService>();
    }

    /// <summary>
    /// Initializes the data by fetching it from the initialization service and adding it to the collections.
    /// </summary>
    public async Task Initialize()
    {
        var users = await InitializerService.GetUsers();
        var roles = await InitializerService.GetRoles();
        var permissions = await InitializerService.GetPermissions();
        var operations = await InitializerService.GetOperations();

        USERS.Value.AddRange(users);
        AUTHORIZATIONS_BASE.Value.AddRange(roles);
        AUTHORIZATIONS_BASE.Value.AddRange(permissions);
        Operations.Value.AddRange(operations);
    }
}