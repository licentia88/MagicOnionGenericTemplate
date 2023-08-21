using Grpc.Core;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Filters;
using MagicT.Server.Jwt;
using MagicT.Server.Services.Base;
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
    public UnaryResult<UserResponse> LoginWithPhoneAsync(LoginRequest loginRequest)
    {
        return ExecuteAsyncWithoutResponse(async () =>
        {
            var user = await FindUserByPhoneAndPasswordAsync(Db, loginRequest.Identifier, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid phone number or password");

            var RWDb = MemoryDatabaseManager.MemoryDatabaseRW();

            var oldTokens = MemoryDatabaseManager.MemoryDatabase.MemoryExpiredTokensTable.FindByContactIdentifier(loginRequest.Identifier);

            RWDb.RemoveMemoryExpiredTokens(oldTokens.Select(x => x.id).ToArray());

            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey, KeyExchangeService.PublicKeyData.privateKey);

            //The diff commands soft updates the values in MemoryDatabase
            RWDb.Diff(new Users[]
            {
                new() { UserId = user.UB_ROWID,  ContactIdentifier = user.U_PHONE_NUMBER, SharedKey = sharedKey }
            });

            //Updates MemoryDatabase
            MemoryDatabaseManager.SaveChanges();

            var token = RequestToken(user.U_PHONE_NUMBER, user.AUTHORIZATIONS_BASE.Select(x => x.AB_ROWID).ToArray());

            return new UserResponse
            {
                Identifier = user.U_PHONE_NUMBER,
                Token = token,
            };
        });
    }

    [Allow]
    public UnaryResult<UserResponse> LoginWithEmailAsync(LoginRequest loginRequest)
    {
        return ExecuteAsyncWithoutResponse(async () =>
        {
            var user = await FindUserByEmailAndPasswordAsync(Db, loginRequest.Identifier, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid Email or password");

            var RWDb = MemoryDatabaseManager.MemoryDatabaseRW();

            var oldTokens = MemoryDatabaseManager.MemoryDatabase.MemoryExpiredTokensTable.FindByContactIdentifier(loginRequest.Identifier);

            RWDb.RemoveMemoryExpiredTokens(oldTokens.Select(x => x.id).ToArray());

            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey, KeyExchangeService.PublicKeyData.privateKey);

            //The diff commands soft updates the values in MemoryDatabase
            RWDb.Diff(new Users[]
            {
                new() { UserId = user.UB_ROWID, ContactIdentifier = user.U_EMAIL, SharedKey = sharedKey }
            });

            //Updates MemoryDatabase
            MemoryDatabaseManager.SaveChanges();

            var token = RequestToken(user.U_EMAIL, user.AUTHORIZATIONS_BASE.Select(x => x.AB_ROWID).ToArray());

            return new UserResponse
            {
                Identifier = user.U_EMAIL,
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
                new() { ContactIdentifier = newUser.U_PHONE_NUMBER, SharedKey = sharedKey }
        });

        //Updates MemoryDatabase
        MemoryDatabaseManager.SaveChanges();

        var token = RequestToken(newUser.U_PHONE_NUMBER);

        return new UserResponse { Identifier = newUser.U_PHONE_NUMBER, Token = token };
    }

    private byte[] RequestToken(string identifier, params int[] roles)
    {
        return MagicTTokenService.CreateToken(identifier, roles);
    }
}