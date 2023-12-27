using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;

namespace MagicT.Shared.Services;

public interface IInitializerService:IService<IInitializerService>
{
    public UnaryResult<List<PERMISSIONS>> GetPermissions();

    public UnaryResult<List<ROLES>> GetRoles();

    public UnaryResult<List<USERS>> GetUsers();

    public UnaryResult<List<Operations>> GetOperations();
}

 