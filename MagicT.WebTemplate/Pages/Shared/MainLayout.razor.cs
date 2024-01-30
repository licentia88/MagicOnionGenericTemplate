﻿using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.WebTemplate.Pages.Shared;

public partial class MainLayout
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [CascadingParameter(Name = nameof(SignOutFunc))]
    public Func<Task> SignOutFunc { get; set; }

    [CascadingParameter(Name = nameof(LoginData))]
    public LoginRequest LoginData { get; set; }
    bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    async Task SignOutAsync()
    {
        await SignOutFunc.Invoke();

        NavigationManager.NavigateTo("/");
    }
}

