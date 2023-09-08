using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.ZoneTree;
using MagicT.Server.ZoneTree.Models;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Filters;

//public class MagicTAuthorizeAttribute : AuthorizationBaseAttribute
//{
//    public int ServiceRole { get; set; }

//    public override ValueTask Invoke(ServiceContext context, int role, Func<ServiceContext, ValueTask> next)
//    {
//        return base.Invoke(context, role, next);
//    }
//}
/// <summary>
/// Custom authorization filter for the MagicOnion framework to validate user roles based on JWT token.
/// </summary>
public  class MagicTAuthorizeAttribute : Attribute, IMagicOnionFilterFactory<IMagicOnionServiceFilter>, IMagicOnionServiceFilter
{
    private int[] Roles { get; }

    private IServiceProvider ServiceProvider { get; set; }

    private MagicTTokenService MagicTTokenService { get; set; }

    private ZoneDbManager ZoneDbManager { get; set; }

    private GlobalData GlobalData { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTAuthorizeAttribute"/> class with the specified roles.
    /// </summary>
    /// <param name="roles">The required roles to access the service methods or hubs.</param>
    public MagicTAuthorizeAttribute(params int[] roles)
    {
        Roles = roles;
    }

   

    /// <summary>
    /// Creates a new instance of the MagicTAuthorize filter using the provided service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <returns>The created MagicTAuthorize filter instance.</returns>
    public IMagicOnionServiceFilter CreateInstance(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        MagicTTokenService = ServiceProvider.GetRequiredService<MagicTTokenService>();

        ZoneDbManager = ServiceProvider.GetRequiredService<ZoneDbManager>();

        GlobalData = serviceProvider.GetRequiredService<GlobalData>();

        return this;
    }

    
    /// <summary>
    /// Invokes the MagicTAuthorize filter logic in the server-side pipeline.
    /// </summary>
    /// <param name="context">The ServiceContext representing the current request context.</param>
    /// <param name="next">The next filter or target method in the pipeline.</param>
    /// <returns>A task representing the asynchronous filter invocation.</returns>
    public async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        var isAllowed = context.AttributeLookup.Any(arg => arg.Key == typeof(AllowAttribute));

        if (!isAllowed)
        {
            //Get Encrypted AuthenticationData Bytes
            var authBytes = context.GetItemFromHeaderAs<byte[]>("crypted-auth-bin");

            //Deserialize it from Bytes to Encrypted AuthenticationData
            var encryptedAuthData = authBytes.DeserializeFromBytes<EncryptedData<AuthenticationData>>();

            //Decrypt to AuthenticationData
            var AuthData = CryptoHelper.DecryptData(encryptedAuthData, GlobalData.Shared);

            var token = ProcessToken(AuthData.Token);

            //if(token.Identifier != AuthData.ContactIdentifier)
            //    throw new ReturnStatusException(StatusCode.Unauthenticated, "Identifiers does not match");
            
            var path = $"{context.ServiceType.Name}/{context.MethodInfo.Name}";
            
            var hasPermission = ZoneDbManager.PermissionsZoneDb.Find(token.Id).Any(x => x == path);

            if (!hasPermission)
                throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));

 
            /**** NOTE ****
             * At this point if data is decrypted successfuly, we know that crypted-auth-bin is not tampered.
             * But an attacker may using this byte[] to make a call request to a different service or from a different user.
             * Because Nonce and Mac are uniqe we will store them in a memory database. 
             * so we will ensure that each crypted-auth-bin can only be used while user having the original token 
             */

            ValidateAuthenticationData(token.Id, encryptedAuthData.EncryptedBytes, encryptedAuthData.Nonce, encryptedAuthData.Mac);
            
            //Add token to ServiceCallContext
            context.AddItem(nameof(MagicTToken), token);

            // ValidateRoles(token, Roles);
        }

        await next(context);
    }

    /// <summary>
    /// Processes the JWT token from the request headers.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="ReturnStatusException"></exception>
    private MagicTToken ProcessToken(byte[] token)
    {
        if (token is null)
            throw new ReturnStatusException(StatusCode.NotFound, "Security Token not found");

        return MagicTTokenService.DecodeToken(token);
    }

    // /// <summary>
    // ///  Validates the roles of the token against the required roles for the service.
    // /// </summary>
    // /// <param name="token"></param>
    // /// <param name="requiredRoles"></param>
    // /// <exception cref="ReturnStatusException"></exception>
    // private void ValidateRoles(MagicTToken token, params int[] requiredRoles)
    // {
    //     // If there are no roles specified for the service, having a valid token grants permission.
    //     if (!Roles.Any())
    //         return;
    //
    //     // Check if the token's roles contain any of the required roles.
    //     // If not, an exception is thrown indicating unauthenticated status.
    //     if (!token.Roles.Any(role => requiredRoles.Contains(role)))
    //         throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
    // }

    
    /// <summary>
    ///  Validates the roles of the token against the required roles for the service.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="encryptedBytes"></param>
    /// <param name="nonce"></param>
    /// <param name="mac"></param>
    /// <exception cref="ReturnStatusException"></exception>
    private void ValidateAuthenticationData(int id,  byte[] encryptedBytes, byte[] nonce, byte[] mac)
    {
        var expiredToken = ZoneDbManager.UsedTokensZoneDb.Find(id)?.Find(x => x.EncryptedBytes == encryptedBytes && x.Nonce == nonce && x.Mac == mac);
        
        if (expiredToken is not null)
            throw new ReturnStatusException(StatusCode.Unauthenticated, "Expired Token");
        
        ZoneDbManager.UsedTokensZoneDb.AddOrUpdate(id, new UsedTokensZone(encryptedBytes, nonce, mac));
    }
}