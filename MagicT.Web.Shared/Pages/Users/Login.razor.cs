using MagicT.Client.Managers;
using MagicT.Client.Services.Base;
using MagicT.Shared.Enums;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Web.Shared.Extensions;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Users;


public partial class Login
{
    

    public AuthenticationRequest AuthenticationRequest { get; set; } = new();
    
    [CascadingParameter(Name = nameof(LoginManager))]
    public LoginManager LoginManager { get; set; }

    [Inject]
    ISecureAuthenticationService Service { get; set; }

    [Inject]
    public IDistributedSubscriber<string, byte[]> subscriber { get; set; }



    public async Task LoginAsync()
    {
        await ExecuteAsync(async () =>
        {
            await LoginManager.CreateAndStoreUserPublics();
            var result = await Service.LoginWithUsernameEncryptedAsync(AuthenticationRequest);

            //token is set in authenticationFilter in clientside
            await LoginManager.SignInAsync(AuthenticationRequest);

            await LoginManager.TokenRefreshSubscriber(AuthenticationRequest);

            NavigationManager.NavigateTo("/");

            return result;

        });
        //     .OnComplete(async result =>
        // {
        //     if (result == TaskResult.Fail)
        //     {
        //         await LoginManager.SignOutAsync();
        //     }
        // });
        
    }

    protected override Task OnBeforeInitializeAsync()
    {
        NavigationManager.NavigateTo("/");
        return base.OnBeforeInitializeAsync();
    }
}
