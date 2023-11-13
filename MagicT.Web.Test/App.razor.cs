using MagicT.Client.Services;
using MagicT.Shared.Models.ServiceModels;
using MagicT.Web.Test.Managers;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Test;

public partial class App
{
    [Inject] private UserManager UserManager { get; set; }

    [Inject] public KeyExchangeService KeyExchangeService { get; set; }

    public (string identifier, EncryptedData<string> securePassword) LoginData { get; set; }

    private byte[] SharedKey { get; set; }

    public bool IsSignedIn => !string.IsNullOrEmpty(LoginData.identifier);

    // public Microsoft.AspNetCore.Components.RouteData NotFoundRouteData =>
    //     new Microsoft.AspNetCore.Components.RouteData(typeof(Login), new Dictionary<string, object>());

    [Inject] public ISubscriber<(string Identifier, EncryptedData<string> SecurePassword)> LoginPublisher { get; set; }


    private bool IsLoaded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await InitializePublicKey();
        await GetLoginDataAsync();


        LoginPublisher.Subscribe(async x =>
        {
            LoginData = await UserManager.GetLoginDataAsync();
            await InvokeAsync(StateHasChanged);
        });


        IsLoaded = true;
        await base.OnInitializedAsync();
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender && !IsSignedIn)
        {
            await GetLoginDataAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetLoginDataAsync()
    {
        LoginData = await UserManager.GetLoginDataAsync();
    }

    private async Task InitializePublicKey()
    {
        await UserManager.LocalStorageService.ClearAsync();

        SharedKey = await UserManager.GetSharedKeyAsync();

        if (SharedKey is not null) return;

        await KeyExchangeService.RequestServerPublicKeyAsync();
    }
}