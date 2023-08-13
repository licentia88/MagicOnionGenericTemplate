using System.Security.Cryptography;
using Grpc.Core;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Filters;
using MagicT.Server.Helpers;
using MagicT.Server.Services.Base;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.MemoryDatabaseModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

[UserFilter]
[MagicTAuthorize]
// ReSharper disable once UnusedType.Global
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class UserService : MagicTServerServiceBase<IUserService, USERS, MagicTContext>, IUserService
{
    
    public UserService(IServiceProvider provider) : base(provider)
    {
    }

    [Allow]
    public  UnaryResult<UserResponse> LoginAsync(LoginRequest loginRequest)
    {
        return  TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            var user = await FindUserByIdAndPasswordAsync(Db, loginRequest.UserId, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid user id or password");

            var token = RequestToken(user.UB_ROWID, user.AUTHORIZATIONS_BASE.Select(x => x.AB_ROWID).ToArray());

            using ECDiffieHellmanCng serverDH = new();

            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            var sharedKey = serverDH.CreateSharedKey(publicKey);

            MemoryDatabase.ToImmutableBuilder().Diff(new Users[]
            {
                new() { UserId = user.UB_ROWID, SharedKey = sharedKey }
            });

            return new UserResponse
            {
                UserId = user.UB_ROWID,
                Token = token,
            };

        });
    }

    [Allow]
    public async UnaryResult<UserResponse> RegisterAsync(RegistrationRequest registrationRequest)
    {

        var userAlreadyExists = await UserIsAlreadyRegistered(Db, registrationRequest.PhoneNumber, registrationRequest.Email);

        if (userAlreadyExists)
            throw new ReturnStatusException(StatusCode.AlreadyExists, "User Already Exists");


        var newUser = new USERS
        {
            U_NAME = registrationRequest.Name,
            U_SURNAME = registrationRequest.Surname,
            U_EMAIL = registrationRequest.Email,
            U_PHONE_NUMBER = registrationRequest.Password,
            UB_PASSWORD = registrationRequest.Password

        };

        await Db.AddAsync(newUser);

        await Db.SaveChangesAsync();

        var token = RequestToken(newUser.UB_ROWID);

         using ECDiffieHellmanCng serverDH = new();

        var publicKey = Context.GetItemAs<byte[]>("public-bin");

        var sharedKey = serverDH.CreateSharedKey(publicKey);

        MemoryDatabase.ToImmutableBuilder().Diff(new Users[]
        {
                new() { UserId = newUser.UB_ROWID, SharedKey = sharedKey }
        });

        return new UserResponse { UserId = newUser.UB_ROWID, Token = token };

    }

    private byte[] RequestToken(int userId, params int [] roles)
    {
        return  MagicTTokenService.CreateToken(userId, roles);
    }
        
        
    /// <summary>
    ///  Find user async Precompiled query
    /// </summary>
    private static readonly Func<MagicTContext, int, string, Task<USERS>> FindUserByIdAndPasswordAsync =    
        EF.CompileAsyncQuery((MagicTContext context, int id, string password) =>
            context.USERS.Include(x=> x.AUTHORIZATIONS_BASE).FirstOrDefault(x => x.UB_ROWID == id && x.UB_PASSWORD == password));
    
    
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
    private static readonly Func<MagicTContext,string, string, Task<bool>> UserIsAlreadyRegistered =
        EF.CompileAsyncQuery((MagicTContext context, string phone, string email) =>
            context.USERS.Any(x => x.U_PHONE_NUMBER == phone || x.U_EMAIL == email ));

}