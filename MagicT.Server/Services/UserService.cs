using Benutomo;
using MagicOnion;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

/// <summary>
/// Service for handling user-related operations.
/// </summary>
// ReSharper disable once UnusedType.Global
public class UserService : MagicServerSecureService<IUserService, USERS, MagicTContext>, IUserService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public UserService(IServiceProvider provider) : base(provider)
    {
    }


    
}