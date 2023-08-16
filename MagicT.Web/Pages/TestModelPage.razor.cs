using MagicT.Shared.Helpers;
using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages;

public sealed partial class TestModelPage
{
    

    [Inject]
    public IKeyExchangeService IDiffieHellmanKeyExchangeService { get; set; }

    [Inject]
    public ILocalStorageService localStorageService { get; set; }

    [Inject]
    public IUserService IUserService { get; set; }

    public async Task Register()
    {
        var newUser = new RegistrationRequest
        {
            Name = "ASIM",
            Surname = "GUNDUZ",
            Password = "1234",
            Email = "a.gunduz@live.com",
            PhoneNumber = "05428502636"

        };
        await IUserService.RegisterAsync(newUser);

    }

    public async Task Login()
    {
        var newUser = new LoginRequest
        {
            UserId =1,
            Password = "1234",
           

        };
        await IUserService.LoginAsync(newUser);

    }
    public async Task GetToken()
    {
        var shaedKey = await localStorageService.GetItemAsync<byte[]>("shared-bin");

        var newTestModel = new TestModel { Description = "test" };

        var cryptedData = await CryptionHelper.EncryptData(newTestModel, shaedKey);

        var decrypted = await CryptionHelper.DecryptData(cryptedData, shaedKey);

        await  Service.CreateEncrypted(cryptedData);
        
    }
}