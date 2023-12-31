using MagicT.Client.Managers;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web;

public partial class App
{
    [Inject]
    private LoginManager LoginManager { get; set; }

    //[Inject]
    //public IKeyExchangeService KeyExchangeService { get; set; }

    private bool IsLoaded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoginManager.StorageManager.ClearAllAsync();
        //Creates and Store Shared and public keys 
        await LoginManager.CreateAndStoreUserPublics();

        await LoginManager.Initialize();

        LoginManager.LoginSubscriber.Subscribe(async x =>
        {
            LoginManager.LoginData = x;
            
            await InvokeAsync(StateHasChanged);
        });
  
        IsLoaded = true;

        await base.OnInitializedAsync();
    }
 
     
}