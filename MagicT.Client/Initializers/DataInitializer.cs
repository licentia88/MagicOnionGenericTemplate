using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Initializers;

// Attribute to register this class as a scoped service
[RegisterScoped]
public class DataInitializer
{
    // Lazy-loaded collections
    public Lazy<List<AUTHORIZATIONS_BASE>> AUTHORIZATIONS_BASE { get; set; }
    public Lazy<List<USERS>> USERS { get; set; }
    public Lazy<List<Operations>> Operations { get; set; }

    // Service for initializing data
    public IInitializerService InitializerService { get; set; }

    // Constructor to inject dependencies
    public DataInitializer(IServiceProvider provider)
    {
        AUTHORIZATIONS_BASE = provider.GetService<Lazy<List<AUTHORIZATIONS_BASE>>>();
        USERS = provider.GetService<Lazy<List<USERS>>>();
        Operations = provider.GetService<Lazy<List<Operations>>>();
        InitializerService = provider.GetService<IInitializerService>();
    }

    // Method to initialize data
    public async Task Initialize()
    {
        // Get data from the initialization service
        var users = await InitializerService.GetUsers();
        var roles = await InitializerService.GetRoles();
        var permissions = await InitializerService.GetPermissions();
        var operations = await InitializerService.GetOperations();

        // Add data to the lazy-loaded collections
        USERS.Value.AddRange(users);
        AUTHORIZATIONS_BASE.Value.AddRange(roles);
        AUTHORIZATIONS_BASE.Value.AddRange(permissions);
        Operations.Value.AddRange(operations);
    }
}