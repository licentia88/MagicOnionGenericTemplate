using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface IUserRolesService : IMagicService<IUserRolesService, USER_ROLES>
{
    UnaryResult<List<USER_ROLES>> FindUserRolesByType(string RoleType);
}
