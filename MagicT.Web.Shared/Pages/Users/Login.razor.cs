using MagicT.Client.Managers;
using MagicT.Shared.Enums;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Web.Shared.Extensions;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Users;


public partial class Login
{
    [CascadingParameter(Name = nameof(LoginData))]
    public (string identifier, EncryptedData<string> securePassword) LoginData { get; set; }

    public LoginRequest LoginRequest { get; set; } = new();

    [Inject] public LoginManager LoginManager { get; set; }

    [Inject]
    IAuthenticationService Service { get; set; }

    [Inject]
    public IDistributedSubscriber<string, byte[]> subscriber { get; set; }



    public async Task LoginAsync()
    {
        await ExecuteAsync(async () =>
        {
            await LoginManager.CreateAndStoreUserPublics();
            var result = await Service.LoginWithUsername(LoginRequest);

            await LoginManager.SignInAsync(LoginRequest);

            await LoginManager.TokenRefreshSubscriber(LoginRequest);

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
