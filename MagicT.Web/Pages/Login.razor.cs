using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Web.Managers;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages;


public partial class Login
{
    [CascadingParameter(Name = nameof(LoginData))]
    public (string identifier, EncryptedData<string> securePassword) LoginData { get; set; }

    public LoginRequest LoginRequest { get; set; } = new();

    [Inject]
    public UserManager UserManager { get; set; }



    public async Task LoginAsync()
    {
        await Service.LoginWithEmailAsync(LoginRequest);

        await UserManager.SignInAsync(LoginRequest);

        NavigationManager.NavigateTo("/");
       
    }
}

