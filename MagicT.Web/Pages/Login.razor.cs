using MagicT.Client.Managers;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages;


public partial class Login
{
    [CascadingParameter(Name = nameof(LoginData))]
    public (string identifier, EncryptedData<string> securePassword) LoginData { get; set; }

    public LoginRequest LoginRequest { get; set; } = new();

    [Inject] public ILoginManager LoginManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    IAuthenticationService Service { get; set; }

  

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        //await ExecuteAsync(RegisterAsync);

    }

    public async Task LoginAsync()
    {
        //await ExecuteAsync(async () =>
        //{
            var result =  await Service.LoginWithEmailAsync(LoginRequest);

            await LoginManager.SignInAsync(LoginRequest);

            NavigationManager.NavigateTo("/");
        //});
    }
}
