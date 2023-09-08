using Grpc.Core;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Filters;
using MagicT.Server.Jwt;
using MagicT.Server.Services.Base;
using MagicT.Server.ZoneTree;
using MagicT.Server.ZoneTree.Models;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
 
            var user = await FindUserByPhoneAsync(Db, loginRequest.Identifier, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid phone number or password");

            var query = $@"
                        SELECT AB_ROWID FROM USERS
                        JOIN USER_ROLES ON UR_USER_REFNO = UB_ROWID
                        JOIN PERMISSIONS ON UR_ROLE_REFNO = PERMISSIONS.PER_ROLE_REFNO WHERE UB_ROWID=@UB_ROWID";


            var roles = GetDatabase().QueryAsync(query, new KeyValuePair<string, object>("U_ROWID", user.UB_ROWID));


            ZoneDbManager.UsedTokensZoneDb.Delete(user.UB_ROWID);
            //ZoneDbManager.ExpiredTokensZoneDb.DeleteBy(x => x.Identifier == loginRequest.Identifier);

            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey, KeyExchangeService.PublicKeyData.privateKey);


            ZoneDbManager.UsersZoneDb.Add(new UsersZone() { UserId= user.UB_ROWID, Identifier = user.U_PHONE_NUMBER, SharedKey = sharedKey });
           
            var rolesAndPermissions = user.USER_ROLES.Select(x => x.UR_ROLE_REFNO).ToArray();

            var token = RequestToken(user.UB_ROWID,user.U_PHONE_NUMBER, rolesAndPermissions);

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
            var user = await FindUserByEmailAsync(Db, loginRequest.Identifier, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid Email or password");

            var query = $@"
                        SELECT * FROM USERS
                        JOIN USER_ROLES ON UR_USER_REFNO = UB_ROWID
                        JOIN PERMISSIONS ON UR_ROLE_REFNO = PERMISSIONS.PER_ROLE_REFNO WHERE UB_ROWID = @UB_ROWID ";



            var roles = Db.USER_ROLES.FirstOrDefault().AUTHORIZATIONS_BASE
            var roles= await GetDatabase().QueryAsync(query, new KeyValuePair<string, object>("U_ROWID", user.UB_ROWID));

            ZoneDbManager.UsedTokensZoneDb.Delete(user.UB_ROWID);

            //ZoneDbManager.ExpiredTokensZoneDb.DeleteBy(x => x.Identifier == loginRequest.Identifier);

            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey, KeyExchangeService.PublicKeyData.privateKey);
 
            ZoneDbManager.UsersZoneDb.Add(new UsersZone() { UserId = user.UB_ROWID, Identifier = user.U_EMAIL, SharedKey = sharedKey });

            int[] rolesAndPermissions = user.USER_ROLES.Select(x => x.UR_ROLE_REFNO).ToArray();
 
            var token = RequestToken(user.UB_ROWID, user.U_EMAIL, rolesAndPermissions);
            
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

        var user = new USERS
        {
            U_NAME = registrationRequest.Name,
            U_SURNAME = registrationRequest.Surname,
            U_EMAIL = registrationRequest.Email,
            U_PHONE_NUMBER = registrationRequest.PhoneNumber,
            UB_PASSWORD = registrationRequest.Password
        };

        await Db.AddAsync(user);

        await Db.SaveChangesAsync();

        //Get Public key from CallContext
        var publicKey = Context.GetItemAs<byte[]>("public-bin");

        if (publicKey is null)
            throw new ReturnStatusException(StatusCode.NotFound, "Key not found");


        var sharedKey = DiffieHellmanKeyExchange.CreateSharedKey(publicKey, KeyExchangeService.PublicKeyData.privateKey);


        ZoneDbManager.UsersZoneDb.Add(new UsersZone() { UserId = user.UB_ROWID, Identifier = user.U_PHONE_NUMBER, SharedKey = sharedKey });

         //Updates MemoryDatabase

        var token = RequestToken(user.UB_ROWID, user.U_EMAIL);


        return new UserResponse { Identifier = user.U_EMAIL, Token = token };
    }

    private byte[] RequestToken(int  Id,string identifier, params int[] roles)
    {
        return MagicTTokenService.CreateToken(Id, identifier, roles);
    }
}