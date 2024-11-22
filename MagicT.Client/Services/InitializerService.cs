using Benutomo;
using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

/// <summary>
/// Preloads Initial Data
/// </summary>
[RegisterScoped]
[AutomaticDisposeImpl]
public partial class InitializerService : MagicClientServiceBase<IInitializerService>, IInitializerService
{
    /// <inheritdoc />
    public InitializerService(IServiceProvider provider) : base(provider)
    {
    }
    
    ~InitializerService()
    {
        Dispose(false);
    }

    /// <summary>
    /// Preloads Operations
    /// </summary>
    /// <returns></returns>
    public UnaryResult<List<Operations>> GetOperations()
    {
        return Client.GetOperations();
    }

    
    /// <summary>
    /// Proloads Permissions
    /// </summary>
    /// <returns></returns>
    public UnaryResult<List<PERMISSIONS>> GetPermissions()
    {
        return Client.GetPermissions();
    }

    /// <summary>
    /// Preloads Roles
    /// </summary>
    /// <returns></returns>
    public UnaryResult<List<ROLES>> GetRoles()
    {
        return Client.GetRoles();
    }

    /// <summary>
    /// Preloads Users
    /// </summary>
    /// <returns></returns>
    public UnaryResult<List<USERS>> GetUsers()
    {
        return Client.GetUsers();
    }
 

    
 
}

