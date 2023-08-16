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

namespace MagicT.Server.Services;

[UserFilter]
[MagicTAuthorize]
public sealed partial class UserService : MagicTServerServiceBase<IUserService, USERS, MagicTContext>, IUserService
{
    public UserService(IServiceProvider provider) : base(provider)
    {
    }

    [Allow]
    public UnaryResult<UserResponse> LoginAsync(LoginRequest loginRequest)
    {
        return TaskHandler.ExecuteAsyncWithoutResponse(async () =>
        {
            var user = await FindUserByIdAndPasswordAsync(Db, loginRequest.UserId, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid user id or password");

            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey);

            //The diff commands soft updates the values in MemoryDatabase
            MemoryDatabaseManager.MemoryDatabaseRW().Diff(new Users[]
            {
                new() { UserId = user.UB_ROWID, SharedKey = sharedKey }
            });

            //Updates MemoryDatabase
            MemoryDatabaseManager.UpdateDatabase();

            var token = RequestToken(user.UB_ROWID, user.AUTHORIZATIONS_BASE.Select(x => x.AB_ROWID).ToArray());

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

        //Get Public key from CallContext
        var publicKey = Context.GetItemAs<byte[]>("public-bin");

        if (publicKey is null)
            throw new ReturnStatusException(StatusCode.NotFound, "Key not found");


        var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey);

        //The diff commands soft updates the values in MemoryDatabase
        MemoryDatabaseManager.MemoryDatabaseRW().Diff(new Users[]
        {
                new() { UserId = newUser.UB_ROWID, SharedKey = sharedKey }
        });

        //Updates MemoryDatabase
        MemoryDatabaseManager.UpdateDatabase();

        var token = RequestToken(newUser.UB_ROWID);

        return new UserResponse { UserId = newUser.UB_ROWID, Token = token };
    }

    private byte[] RequestToken(int userId, params int[] roles)
    {
        return MagicTTokenService.CreateToken(userId, roles);
    }
}