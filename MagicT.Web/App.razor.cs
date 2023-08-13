using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web;

public partial class App
{
    [Inject]
    private ILocalStorageService LocalStorageService { get; set; }

    [Inject]
    private IKeyExchangeService KeyExchangeService { get; set; }

    private byte[] SharedKey { get; set; }

    protected override async Task OnInitializedAsync()
    { 
        await InitializePublicKey();
        await base.OnInitializedAsync();
    }
 
    private async Task InitializePublicKey()
    {
        SharedKey = await LocalStorageService.GetItemAsync<byte[]>("shared-bin");

        if (SharedKey is not null) return;

        await KeyExchangeService.RequestServerPublicKeyAsync();


    }
}