using MagicOnion;
using MagicT.Shared.Models;
using MagicT.Shared.Services.Base;

namespace MagicT.Shared.Services;

public interface IUserRolesService : ISecureMagicService<IUserRolesService, USER_ROLES>
{
    UnaryResult<List<USER_ROLES>> FindUserRolesByType(int userId, string RoleType);
}
