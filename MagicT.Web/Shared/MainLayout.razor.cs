using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared;

public partial class MainLayout
{
    [CascadingParameter(Name = nameof(SignOutFunc))]
    public Func<Task> SignOutFunc { get; set; }

    [CascadingParameter(Name =nameof(LoginData))]
    public LoginRequest LoginData { get; set; }
    bool _drawerOpen = true;
 
    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    async Task SignOutAsync()
    {
        await SignOutFunc.Invoke();
    }
}

