using System.Reflection;
using MagicT.Client.Managers;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web;

public partial class App
{
     
    [Inject]
    private LoginManager LoginManager { get; set; }

 
    [Parameter]
    public bool IsAdminPath { get; set; }
    
    private Func<Task> SignOutFunc { get; set; }
    
    private bool IsLoaded { get; set; }
    

    protected override async Task OnInitializedAsync()
    {
        SignOutFunc = SignOutAsync;
        
        await LoginManager.LoadCacheData();

        LoginManager.LoginSubscriber.Subscribe(async x =>
        {
            LoginManager.LoginData = x;
            
            await InvokeAsync(StateHasChanged);
        });
  
        IsLoaded = true;

        await base.OnInitializedAsync();
    }

    public async Task SignOutAsync()
    {
        await LoginManager.SignOutAsync();

        StateHasChanged();
    }


}