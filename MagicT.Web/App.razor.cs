using MagicT.Shared.Services;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

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