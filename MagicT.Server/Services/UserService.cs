using Grpc.Core;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Filters;
using MagicT.Server.Jwt;
using MagicT.Server.Services.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.MemoryDatabaseModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

[KeyExchangeFilter]
public sealed partial class UserService : AuthorizationSeviceBase<IUserService, USERS, MagicTContext>, IUserService
{
    public KeyExchangeService KeyExchangeService { get; set; }

    public MagicTTokenService MagicTTokenService { get; set; }

    public UserService(IServiceProvider provider) : base(provider)
    {
        MagicTTokenService = provider.GetService<MagicTTokenService>();
        KeyExchangeService = provider.GetService<KeyExchangeService>();
    }


    /// <summary>
    /// Logs in a user with the provided login credentials and returns a user response with a token.
    /// </summary>
    /// <param name="loginRequest">The login request containing user credentials.</param>
    /// <returns>A user response containing user information and a token.</returns>
    [Allow]
    public UnaryResult<UserResponse> LoginAsync(LoginRequest loginRequest)
    {
        return ExecuteAsyncWithoutResponse(async () =>
        {
            var user = await FindUserByIdAndPasswordAsync(Db, loginRequest.UserId, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid user id or password");

            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey, KeyExchangeService.PublicKeyData.privateKey);

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


    /// <summary>
    /// Registers a new user with the provided registration information and returns a user response with a token.
    /// </summary>
    /// <param name="registrationRequest">The registration request containing user information.</param>
    /// <returns>A user response containing user information and a token.</returns>
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


        var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey, KeyExchangeService.PublicKeyData.privateKey);

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