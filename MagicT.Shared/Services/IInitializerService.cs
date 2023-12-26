using MagicOnion;
using MagicT.Shared.Models;

namespace MagicT.Shared.Services;

public interface IInitializerService:IService<IInitializerService>
{
    public UnaryResult<List<PERMISSIONS>> GetPermissions();

    public UnaryResult<List<ROLES>> GetRoles();

    public UnaryResult<List<USERS>> GetUsers();
}

 