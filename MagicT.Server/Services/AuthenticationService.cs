﻿using System.Text;
using AQueryMaker.Extensions;
using Grpc.Core;
using MagicOnion;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Filters;
using MagicT.Server.Jwt;
using MagicT.Server.Models;
using MagicT.Server.Services.Base;
using MagicT.Shared.Managers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Services;

[AuthenticationFilter]
public sealed class AuthenticationService : MagicServerBase<IAuthenticationService>, IAuthenticationService
{
    public IKeyExchangeManager KeyExchangeManager { get; set; }

    public MagicTTokenService MagicTTokenService { get; set; }

    public MagicTContext Db { get; set; }
    public AuthenticationService(IServiceProvider provider) : base(provider)
    {
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();
        MagicTTokenService = provider.GetService<MagicTTokenService>();
        Db = provider.GetService<MagicTContext>();
    }


    /// <summary>
    /// Logs in a user with the provided login credentials and returns a user response with a token.
    /// </summary>
    /// <param name="loginRequest">The login request containing user credentials.</param>
    /// <returns>A user response containing user information and a token.</returns>
    [Allow]
    public async UnaryResult<LoginResponse> LoginWithPhoneAsync(LoginRequest loginRequest)
    {
        return await ExecuteAsync(async () =>
        {
            var user = await FindUserByPhoneAsync(Db, loginRequest.Identifier, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid phone number or password");

            const string query = $@"
                        SELECT AB_ROWID FROM USERS
                        JOIN USER_ROLES ON UR_USER_REFNO = UB_ROWID
                        JOIN PERMISSIONS ON UR_ROLE_REFNO = PERMISSIONS.PER_ROLE_REFNO WHERE UB_ROWID=@UB_ROWID";


            var roles = Db.SqlManager().QueryAsync(query, new KeyValuePair<string, object>("U_ROWID", user.UB_ROWID));

                  
            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = KeyExchangeManager.CreateSharedKey(publicKey, KeyExchangeManager.KeyExchangeData.PrivateKey);

 
            MagicTRedisDatabase.AddOrUpdate(Convert.ToString(user.UB_ROWID), new UsersCredentials
            {
                UserId = user.UB_ROWID, Identifier= user.U_PHONE_NUMBER, SharedKey=sharedKey
            });

            var rolesAndPermissions = user.USER_ROLES.Select(x => x.UR_ROLE_REFNO).ToArray();

            var token = MagicTTokenService.CreateToken(user.UB_ROWID, user.U_PHONE_NUMBER, rolesAndPermissions);

            return new LoginResponse
            {
                Identifier = user.U_PHONE_NUMBER,
                Token = token,
            };
        });
    }

    [Allow]
    public async UnaryResult<LoginResponse> LoginWithEmailAsync(LoginRequest loginRequest)
    {
        return await ExecuteAsync(async () =>
        {
            var user = await FindUserByEmailAsync(Db, loginRequest.Identifier, loginRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid Email or password");

            //Get Public key from CallContext
            var publicKey = Context.GetItemAs<byte[]>("public-bin");

            var publicKeyString = Encoding.UTF8.GetString(publicKey);

            if (publicKey is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

            //Use DiffieHellman to Create Shared Key
            var sharedKey = KeyExchangeManager.CreateSharedKey(publicKey, KeyExchangeManager.KeyExchangeData.PrivateKey);

            MagicTRedisDatabase.AddOrUpdate(Convert.ToString(user.UB_ROWID), new UsersCredentials
            {
                UserId = user.UB_ROWID, Identifier = user.U_EMAIL, SharedKey = sharedKey
            });
          
            var rolesAndPermissions = user.USER_ROLES.Select(x => x.UR_ROLE_REFNO).ToArray();

            var token = MagicTTokenService.CreateToken(user.UB_ROWID, user.U_EMAIL, rolesAndPermissions);

            return new LoginResponse
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
    public async UnaryResult<LoginResponse> RegisterAsync(RegistrationRequest registrationRequest)
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


        var sharedKey = KeyExchangeManager.CreateSharedKey(publicKey, KeyExchangeManager.KeyExchangeData.PrivateKey);
 

        MagicTRedisDatabase.Create(Convert.ToString(user.UB_ROWID), new UsersCredentials
        {
            UserId = user.UB_ROWID, Identifier = user.U_PHONE_NUMBER, SharedKey = sharedKey
        });

        var token = MagicTTokenService.CreateToken(user.UB_ROWID, user.U_EMAIL);

        return new LoginResponse { Identifier = user.U_EMAIL, Token = token };
    }

    //private byte[] RequestToken(int Id, string identifier, params int[] roles)
    //{
    //    return MagicTTokenService.CreateToken(Id, identifier, roles);
    //}

    /// <summary>
    ///  Find user async Precompiled query
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<USERS>> FindUserByPhoneAsync =
        EF.CompileAsyncQuery((MagicTContext context, string phonenumber, string password) =>
            context.USERS.Include(x => x.USER_ROLES).ThenInclude(x => x.AUTHORIZATIONS_BASE).FirstOrDefault(x => x.U_PHONE_NUMBER == phonenumber && x.UB_PASSWORD == password));


    /// <summary>
    ///  Find user async Precompiled query
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<USERS>> FindUserByEmailAsync =
        EF.CompileAsyncQuery((MagicTContext context, string email, string password) =>
            context.USERS.Include(x => x.USER_ROLES).ThenInclude(x => x.AUTHORIZATIONS_BASE).FirstOrDefault(x => x.U_EMAIL == email && x.UB_PASSWORD == password));



    /// <summary>
    /// Checks wheter user is register via email or phone number
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<bool>> UserIsAlreadyRegistered =
        EF.CompileAsyncQuery((MagicTContext context, string phone, string email) =>
            context.USERS.Any(x => x.U_PHONE_NUMBER == phone || x.U_EMAIL == email));


}
