using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Filters;


public class MagicTAuthorizeAttribute : Attribute, IMagicOnionFilterFactory<IMagicOnionServiceFilter>, IMagicOnionServiceFilter
{
    private KeyExchangeData GlobalData { get; set; }

    public AuthenticationManager AuthenticationManager { get; set; }

    public TokenManager TokenManager { get; set; }

    public MagicTAuthorizeAttribute()
    {
    }

   

    /// <summary>
    /// Creates a new instance of the MagicTAuthorize filter using the provided service provider.
    /// </summary>
    /// <param name="provider">The service provider for resolving dependencies.</param>
    /// <returns>The created MagicTAuthorize filter instance.</returns>
    public IMagicOnionServiceFilter CreateInstance(IServiceProvider provider)
    {
        GlobalData = provider.GetService<KeyExchangeData>();

        AuthenticationManager = provider.GetService<AuthenticationManager>();

        TokenManager = provider.GetService<TokenManager>();

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

            if (authBytes is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Token not found");

            //Deserialize it from Bytes to Encrypted AuthenticationData
            EncryptedData<AuthenticationData> encryptedAuthData = authBytes.DeserializeFromBytes<EncryptedData<AuthenticationData>>();

            //Decrypt to AuthenticationData
            var AuthData = CryptoHelper.DecryptData(encryptedAuthData, GlobalData.SharedBytes);

            var token = TokenManager.Process(AuthData.Token);

            if (token.Identifier.ToLower() != AuthData.ContactIdentifier.ToLower())
                throw new ReturnStatusException(StatusCode.Unauthenticated, "Identifiers does not match");

            AuthenticationManager.AuthenticateData(token.Id, encryptedAuthData);

            var endPoint = $"{context.ServiceType.Name}/{context.MethodInfo.Name}";

            AuthenticationManager.ValidateRoles(token, endPoint);
 
            /**** NOTE ****
             * At this point if data is decrypted successfuly, we know that crypted-auth-bin is not tampered.
             * But an attacker may using this byte[] to make a call request to a different service or from a different user.
             * Because Nonce and Mac are uniqe we will store them in a memory database. 
             * so we will ensure that each crypted-auth-bin can only be used while user having the original token 
             */

            //Add token to ServiceCallContext
            context.AddItem(nameof(MagicTToken), token);

        }

     

        await next(context);
    }

   
 
 
   
}