using Blazored.LocalStorage;
using MagicT.Shared.Helpers;
using MagicT.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages.Examples;

public sealed partial class TestModelService
{
    [Inject]
    public ITestService TestService { get; set; }

    [Inject]
    ILocalStorageService storageService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var sharedbin = await storageService.GetItemAsync<byte[]>("shared-bin");

        var encryptedData = CryptoHelper.EncryptData("ASIM", sharedbin);

        var test = await Service.EncryptedString(encryptedData);

         await base.OnInitializedAsync();
    }
}

 