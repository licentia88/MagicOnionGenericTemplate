using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using MagicT.Web.Test.Managers;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Test.Pages;


public partial class Login
{
    [CascadingParameter(Name = nameof(LoginData))]
    public (string identifier, EncryptedData<string> securePassword) LoginData { get; set; }

    public LoginRequest LoginRequest { get; set; } = new();

    [Inject] public UserManager UserManager { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    IAuthenticationService Service { get; set; }

    public async Task RegisterAsync()
    {
        var newreg = new RegistrationRequest
        {
            Email = "a.gunduz@live.com",
            Name = "ASIM",
            Surname = "GUNDUZ",
            Password = "224450",
            PhoneNumber = "05428502636"
        };

        await Service.RegisterAsync(newreg);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        //await ExecuteAsync(RegisterAsync);

    }

    public async Task LoginAsync()
    {
        //await ExecuteAsync(async () =>
        //{
        await Service.LoginWithEmailAsync(LoginRequest);

        await UserManager.SignInAsync(LoginRequest);


        NavigationManager.NavigateTo("/");
        //});
    }
}
