using Grpc.Core;
using MagicOnion.Client;
using MagicT.Client.Exceptions;
using MagicT.Client.Extensions;
using MagicT.Client.Models;
using MagicT.Shared.Extensions;
using MagicT.Shared.Helpers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
///  This filter is responsible for sending Client public keys to the server
/// </summary>
public sealed  class UserAuthenticationFilter : IClientFilter
{
    public GlobalData GlobalData { get; set; }

    /// <summary>
    /// Local storage service
    /// </summary>
    public ILocalStorageService LocalStorageService { get; set; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public UserAuthenticationFilter(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ILocalStorageService>();
        GlobalData = provider.GetService<GlobalData>();
    }

    /// <inheritdoc/>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {
        //If login or register methods send publickey to server to create shared key in server

        if (context.MethodPath == $"{nameof(IUserService)}/{nameof(IUserService.LoginWithPhoneAsync)}" ||
            context.MethodPath == $"{nameof(IUserService)}/{nameof(IUserService.LoginWithEmailAsync)}" ||
            context.MethodPath == $"{nameof(IUserService)}/{nameof(IUserService.RegisterAsync)}")
        {
            var publicKey = await LocalStorageService.GetItemAsync<byte[]>("public-bin");

            context.CallOptions.Headers.AddorUpdateItem("public-bin", publicKey);

            var response = await next(context);

            //Get UserResponse 
            var userResponse = await response.GetResponseAs<UserResponse>();

            //Set Token to localStorage
            await LocalStorageService.SetItemAsync("token-bin", userResponse.Token);

            return response;
        }

        var contactIdentfier = await LocalStorageService.GetItemAsync<string>("Identifier");

        var token = await LocalStorageService.GetItemAsync<byte[]>("token-bin");

        var authData = new AuthenticationData(token, contactIdentfier);

        var cyptedAuthData = CryptoHelper.EncryptData(authData, GlobalData.Shared);

        var cryptedAuthBin = cyptedAuthData.SerializeToBytes();

        if (token is null)
            throw new AuthException(StatusCode.NotFound, "Security Token not found");

        context.CallOptions.Headers.AddorUpdateItem("crypted-auth-bin", cryptedAuthBin);

        return await next(context);
       
    }
}
