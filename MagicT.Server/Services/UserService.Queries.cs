using MagicT.Server.Database;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

public partial class UserService
{ 
    /// <summary>
    ///  Find user async Precompiled query
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<USERS>> FindUserByPhoneAndPasswordAsync =
        EF.CompileAsyncQuery((MagicTContext context, string phonenumber, string password) =>
            context.USERS.Include(x => x.USER_AUTHORIZATIONS).FirstOrDefault(x => x.U_PHONE_NUMBER == phonenumber && x.UB_PASSWORD == password));

    
    /// <summary>
    ///  Find user async Precompiled query
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<USERS>> FindUserByEmailAndPasswordAsync =
        EF.CompileAsyncQuery((MagicTContext context, string email, string password) =>
            context.USERS.Include(x => x.USER_AUTHORIZATIONS).FirstOrDefault(x => x.U_EMAIL == email && x.UB_PASSWORD == password));

    /// <summary>
    /// Find user by phone number async Precompiled query
    /// </summary>
    private static readonly Func<MagicTContext, string, Task<USERS>> FindUserByPhoneNumber =
        EF.CompileAsyncQuery((MagicTContext context, string phoneNumber) =>
            context.USERS.FirstOrDefault(x => x.U_PHONE_NUMBER == phoneNumber));

    /// <summary>
    /// Find user by email async Precompiled query
    /// </summary>
    private static readonly Func<MagicTContext, string, Task<USERS>> FindUserByEmail =
        EF.CompileAsyncQuery((MagicTContext context, string email) =>
            context.USERS.FirstOrDefault(x => x.U_EMAIL == email));

    /// <summary>
    /// Checks wheter user is register via email or phone number
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<bool>> UserIsAlreadyRegistered =
        EF.CompileAsyncQuery((MagicTContext context, string phone, string email) =>
            context.USERS.Any(x => x.U_PHONE_NUMBER == phone || x.U_EMAIL == email));
}
