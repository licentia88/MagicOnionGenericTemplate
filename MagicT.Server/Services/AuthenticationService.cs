using Grpc.Core;
using MagicOnion;
using MagicT.Server.Extensions;
using MagicT.Server.Filters;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Server.Models;
using MagicT.Server.Services.Base;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Managers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;
/// <summary>
/// Service for handling authentication operations.
/// </summary>
[AuthenticationFilter]
// ReSharper disable once UnusedType.Global
public sealed class AuthenticationService : MagicServerBase<IAuthenticationService>, IAuthenticationService
{
    private IKeyExchangeManager KeyExchangeManager { get; }
  
    // [EnableAutomaticDispose]
    private TokenManager TokenManager { get; }
    
    // [EnableAutomaticDispose]
    private MagicTContext Db { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuthenticationService(IServiceProvider provider) : base(provider)
    {
        KeyExchangeManager = provider.GetService<IKeyExchangeManager>();
        TokenManager = provider.GetService<TokenManager>();
        Db = provider.GetService<MagicTContext>();
    }

    ~AuthenticationService()
    {
        Dispose(false);
    }
    /// <summary>
    /// Logs in a user with the provided phone number and returns a user response with a token.
    /// </summary>
    /// <param name="authenticationRequest">The login request containing user credentials.</param>
    /// <returns>A user response containing user information and a token.</returns>
    [Allow]
    public async UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithPhoneAsync(EncryptedData<AuthenticationRequest> authenticationRequest)
    {
        return await ExecuteLoginAsync(authenticationRequest, FindByPhoneAsync);
    }

    /// <summary>
    /// Logs in a user with the provided email and returns a user response with a token.
    /// </summary>
    /// <param name="authenticationRequest">The login request containing user credentials.</param>
    /// <returns>A user response containing user information and a token.</returns>
    [Allow]
    public async UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithEmailAsync(EncryptedData<AuthenticationRequest> authenticationRequest)
    {
        return await ExecuteLoginAsync(authenticationRequest, FindByEmailAsync);
    }

    /// <summary>
    /// Logs in a user with the provided username and returns a user response with a token.
    /// </summary>
    /// <param name="authenticationRequest">The login request containing user credentials.</param>
    /// <returns>A user response containing user information and a token.</returns>
    [Allow]
    public async UnaryResult<EncryptedData<AuthenticationResponse>> LoginWithUsername(EncryptedData<AuthenticationRequest> authenticationRequest)
    {
        return await ExecuteLoginAsync(authenticationRequest, FindByUsernameAsync);
    }

    /// <summary>
    /// Registers a new user with the provided registration information and returns a user response with a token.
    /// </summary>
    /// <param name="registrationRequest">The registration request containing user information.</param>
    /// <returns>A user response containing user information and a token.</returns>
    [Allow]
    public async UnaryResult<EncryptedData<AuthenticationResponse>> RegisterAsync(EncryptedData<RegistrationRequest> registrationRequest)
    {
        // ServerSharedkey
        var serverSharedKey =KeyExchangeManager.KeyExchangeData.SharedBytes;
            
        // Decrypt the encrypted data.
        var decryptedRequest = CryptoHelper.DecryptData(registrationRequest, serverSharedKey);
        
        var userAlreadyExists = await UserIsAlreadyRegistered(Db, decryptedRequest.PhoneNumber, decryptedRequest.Email);

        if (userAlreadyExists)
            throw new ReturnStatusException(StatusCode.AlreadyExists, "User Already Exists");

        var user = new USERS
        {
            U_NAME = decryptedRequest.Name,
            U_LASTNAME = decryptedRequest.Lastname,
            U_EMAIL = decryptedRequest.Email,
            U_PHONE_NUMBER = decryptedRequest.PhoneNumber,
            U_PASSWORD = decryptedRequest.Password
        };

        
        var publicKey = GetPublicKeyFromContext();
        var userShared = KeyExchangeManager.CreateSharedKey(publicKey, KeyExchangeManager.KeyExchangeData.PrivateKey);
        
        await Db.AddAsync(user);
        await Db.SaveChangesAsync();
 
        await MagicTRedisDatabase.CreateAsync(Convert.ToString(user.U_ROWID), new UsersCredentials
        {
            UserId = user.U_ROWID,
            Identifier = user.U_PHONE_NUMBER,
            SharedKey = userShared
        });

        var token = TokenManager.CreateToken(user.U_ROWID, user.U_EMAIL);

        var authenticationResponse = new AuthenticationResponse(token, user.U_EMAIL);
        

        return CryptoHelper.EncryptData(authenticationResponse, userShared);
    }

    /// <summary>
    /// Executes the login process for a user.
    /// </summary>
    /// <param name="authenticationRequest">The login request containing user credentials.</param>
    /// <param name="findUserAsync">The function to find the user asynchronously.</param>
    /// <returns>A user response containing user information and a token.</returns>
    private async Task<EncryptedData<AuthenticationResponse>> ExecuteLoginAsync(EncryptedData<AuthenticationRequest> authenticationRequest, Func<MagicTContext, string, string, Task<USERS>> findUserAsync)
    {
        return await ExecuteAsync(async () =>
        {
            // ServerSharedkey
            var serverSharedKey =KeyExchangeManager.KeyExchangeData.SharedBytes;
            
            // Decrypt the encrypted data.
            var decryptedRequest = CryptoHelper.DecryptData(authenticationRequest, serverSharedKey);
            
            var user = await findUserAsync(Db, decryptedRequest.Identifier, decryptedRequest.Password);

            if (user is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Invalid credentials");

            var publicKey = GetPublicKeyFromContext();
            var userShared = KeyExchangeManager.CreateSharedKey(publicKey, KeyExchangeManager.KeyExchangeData.PrivateKey);

            await MagicTRedisDatabase.AddOrUpdateAsync(Convert.ToString(user.U_ROWID), new UsersCredentials
            {
                UserId = user.U_ROWID,
                Identifier = decryptedRequest.Identifier,
                SharedKey = userShared
            });

            var rolesAndPermissions = user.USER_ROLES.Select(x => x.UR_ROLE_REFNO).ToArray();
            var token = TokenManager.CreateToken(user.U_ROWID, decryptedRequest.Identifier, rolesAndPermissions);

            var authenticationResponse =new AuthenticationResponse(token, decryptedRequest.Identifier);
            

            return CryptoHelper.EncryptData(authenticationResponse, userShared);
             
        });
    }

    /// <summary>
    /// Gets the public key from the call context.
    /// </summary>
    /// <returns>The public key as a byte array.</returns>
    private byte[] GetPublicKeyFromContext()
    {
        var publicKey = Context.GetItemAs<byte[]>("public-bin");

        if (publicKey is null)
            throw new ReturnStatusException(StatusCode.NotFound, "Key not found");

        return publicKey;
    }

    /// <summary>
    /// Finds a user by phone number asynchronously.
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<USERS>> FindByPhoneAsync =
        EF.CompileAsyncQuery((MagicTContext context, string phonenumber, string password) =>
            context.USERS.Include(x => x.USER_ROLES).ThenInclude(x => x.AUTHORIZATIONS_BASE).FirstOrDefault(x => x.U_PHONE_NUMBER == phonenumber && x.U_PASSWORD == password));

    /// <summary>
    /// Finds a user by email asynchronously.
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<USERS>> FindByEmailAsync =
        EF.CompileAsyncQuery((MagicTContext context, string email, string password) =>
            context.USERS.Include(x => x.USER_ROLES).ThenInclude(x => x.AUTHORIZATIONS_BASE).FirstOrDefault(x => x.U_EMAIL == email && x.U_PASSWORD == password));

    /// <summary>
    /// Finds a user by username asynchronously.
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<USERS>> FindByUsernameAsync =
        EF.CompileAsyncQuery((MagicTContext context, string username, string password) =>
            context.USERS.Include(x => x.USER_ROLES).ThenInclude(x => x.AUTHORIZATIONS_BASE).FirstOrDefault(x => x.U_USERNAME == username && x.U_PASSWORD == password));

    /// <summary>
    /// Checks whether a user is already registered via email or phone number.
    /// </summary>
    private static readonly Func<MagicTContext, string, string, Task<bool>> UserIsAlreadyRegistered =
        EF.CompileAsyncQuery((MagicTContext context, string phone, string email) =>
            context.USERS.Any(x => x.U_PHONE_NUMBER == phone || x.U_EMAIL == email));
}
