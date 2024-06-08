using System.Reflection;
using MagicT.Client.Managers;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web;

public partial class App
{
     
    [Inject]
    private LoginManager LoginManager { get; set; }

    private Func<Task> SignOutFunc { get; set; }

    public Action ThemeToggled { get; set; }

    private bool IsLoaded { get; set; }

    public bool IsDarkMode { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SignOutFunc = SignOutAsync;
        ThemeToggled = OnToggleTheme;

        //await LoginManager.StorageManager.ClearAllAsync();
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

    public async Task SignOutAsync()
    {
        await LoginManager.SignOutAsync();

        this.StateHasChanged();
    }

    public void OnToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        StateHasChanged();
    }
}