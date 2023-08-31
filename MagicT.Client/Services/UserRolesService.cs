﻿using MagicT.Client.Filters;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

public sealed class UserRolesService : MagicClientSecureServiceBase<IUserRolesService, USER_ROLES>, IUserRolesService
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public UserRolesService(IServiceProvider provider) : base(provider)
    {
    }
}

