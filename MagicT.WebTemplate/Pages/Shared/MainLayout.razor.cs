﻿using MagicT.Client.Managers;
using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MagicT.WebTemplate.Pages.Shared;

public partial class MainLayout
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [CascadingParameter(Name = nameof(SignOutFunc))]
    public Func<Task> SignOutFunc { get; set; }

    [CascadingParameter(Name = nameof(LoginManager))]
    public LoginManager LoginManager { get; set; }

    [CascadingParameter(Name = nameof(IsDarkMode))]
    public bool IsDarkMode { get; set; }

    [CascadingParameter(Name = nameof(ThemeToggled))]
    public Action ThemeToggled { get; set; }

    public MudTheme CurrentTheme { get; set; } = new();

    bool _drawerOpen = true;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    public void OnThemeToggled()
    {
        ThemeToggled.Invoke();
    }
    async Task SignOutAsync()
    {
        await SignOutFunc.Invoke();

        NavigationManager.NavigateTo("/");
    }
}

