using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages;

public partial class TestModelPage
{
    [Inject]
    public  ITokenService tokenService { get; set; }

    [Inject]
    public IDiffieHellmanKeyExchangeService IDiffieHellmanKeyExchangeService { get; set; }
    //[Inject]
    //ICookieService CookieService { get; set; }



    [Inject]
    public ILocalStorageService localStorageService { get; set; }


    public async Task GetToken()
    {
        //await IDiffieHellmanKeyExchangeService.InitializeKeyExchangeAsync();

        var response = await tokenService.Request(1, "222");

        //await CookieService.SetValue("auth-token-bin", "test");

        //var result = await CookieService.GetValue("auth-token-bin");
 
        await localStorageService.SetItemAsync<byte[]>("auth-token-bin", response);
    }
}