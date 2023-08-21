using MagicOnion.Client;
using MagicT.Client.Extensions;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.Extensions.DependencyInjection;

namespace MagicT.Client.Filters;

/// <summary>
///  Filter for adding user information to gRPC client requests.
/// </summary>
public sealed  class UserServiceFilter : IClientFilter
{
    /// <summary>
    /// Local storage service
    /// </summary>
    public ILocalStorageService LocalStorageService { get; set; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="provider"></param>
    public UserServiceFilter(IServiceProvider provider)
    {
        LocalStorageService = provider.GetService<ILocalStorageService>();
    }

    /// <inheritdoc/>
    public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
    {  
        if (context.MethodPath != $"{nameof(IUserService)}/{nameof(IUserService.LoginWithPhoneAsync)}" &&
            context.MethodPath != $"{nameof(IUserService)}/{nameof(IUserService.LoginWithEmailAsync)}" &&
            context.MethodPath != $"{nameof(IUserService)}/{nameof(IUserService.RegisterAsync)}")
            return await next(context);
         
        //If login or register methods send publickey to server to create shared key in server
        var publicKey = await LocalStorageService.GetItemAsync<byte[]>("public-bin");

        context.CallOptions.Headers.AddorUpdateItem("public-bin", publicKey);

        var response = await next(context);

        //Get UserResponse 
        var userResponse = await response.GetResponseAs<UserResponse>();

        //Set Token to localStorage
        await  LocalStorageService.SetItemAsync("token-bin", userResponse.Token);

        return response;
    }
}
