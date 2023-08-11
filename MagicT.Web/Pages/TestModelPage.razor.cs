using MagicT.Client.Services;
using MagicT.Shared.Models.MemoryDatabaseModels;
using MagicT.Shared.Models.ServiceModels;
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

    public IUserService IUserService { get; set; }

    public async Task Register()
    {
        var newUser = new RegistrationRequest
        {
            Name = "ASIM",
            Surname = "GUNDUZ", Password = "1234", Email = "a.gunduz@live.com", PhoneNumber = "05428502636"

        };
        await IUserService.RegisterAsync(newUser);

    }
    public async Task GetToken()
    {
        //var tt = await Service.CreateEncrypted(new EncryptedData(null, null, null));


        //await IDiffieHellmanKeyExchangeService.InitializeKeyExchangeAsync();

        // var response = await tokenService.Request(1, "222");

        //await CookieService.SetValue("auth-token-bin", "test");

        //var result = await CookieService.GetValue("auth-token-bin");
 
        // await localStorageService.SetItemAsync<byte[]>("auth-token-bin", response);
    }
}