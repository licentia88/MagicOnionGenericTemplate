using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using MagicT.Server.Extensions;
using MagicT.Server.Jwt;
using MagicT.Server.Managers;
using MagicT.Shared.Cryptography;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models.ServiceModels;

namespace MagicT.Server.Filters;


/// <summary>
/// A custom authorization attribute for the MagicOnion framework.
/// This attribute handles the authorization logic for service methods.
/// </summary>
public class AuthorizeAttribute : Attribute, IMagicOnionFilterFactory<IMagicOnionServiceFilter>, IMagicOnionServiceFilter
{
    /// <summary>
    /// Gets or sets the global key exchange data.
    /// </summary>
    private KeyExchangeData GlobalData { get; set; }

    /// <summary>
    /// Gets or sets the authentication manager.
    /// </summary>
    private AuthenticationManager AuthenticationManager { get; set; }

    /// <summary>
    /// Gets or sets the token manager.
    /// </summary>
    private TokenManager TokenManager { get; set; }

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
            // Get Encrypted AuthenticationData Bytes
            var authBytes = context.GetItemFromHeaderAs<byte[]>("crypted-auth-bin");

            if (authBytes is null)
                throw new ReturnStatusException(StatusCode.NotFound, "Token not found");

            // Deserialize it from Bytes to Encrypted AuthenticationData
            EncryptedData<AuthenticationData> encryptedAuthData = authBytes.DeserializeFromBytes<EncryptedData<AuthenticationData>>();

            // Decrypt to AuthenticationData
            var authData = CryptoHelper.DecryptData(encryptedAuthData, GlobalData.SharedBytes);

            var token = TokenManager.Process(authData.Token);

            if (token.Identifier.ToLower() != authData.ContactIdentifier.ToLower())
                throw new ReturnStatusException(StatusCode.Unauthenticated, "Identifiers does not match");

            AuthenticationManager.AuthenticateData(token.Id, encryptedAuthData);

            var endPoint = $"{context.ServiceType.Name}/{context.MethodInfo.Name}";

            AuthenticationManager.ValidateRoles(token, endPoint);

            // Add token to ServiceCallContext
            context.AddItem(nameof(MagicTToken), token);
        }

        await next(context);
    }
}