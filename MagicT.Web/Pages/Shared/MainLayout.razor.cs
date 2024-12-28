using MagicT.Client.Managers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.Web.Pages.Shared;

public partial class MainLayout
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    
    [CascadingParameter(Name = nameof(LoginManager))]
    private LoginManager LoginManager { get; set; }
    
    public MudTheme CurrentTheme { get; set; } = new();

    bool _drawerOpen = true;
    
    private bool IsLoaded { get; set; }
    public bool IsDarkMode { get; set; }
    
    [CascadingParameter(Name = nameof(SignOutFunc))]
    private Func<Task> SignOutFunc { get; set; }

    public async Task SignOutAsync()
    {
         NavigationManager.NavigateTo("/");

        await SignOutFunc();

        // this.StateHasChanged();
    }

    public void OnToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        StateHasChanged();
    }
    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
}

