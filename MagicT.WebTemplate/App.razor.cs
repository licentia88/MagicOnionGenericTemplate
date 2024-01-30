﻿using System;
using MagicT.Client.Managers;
using MessagePipe;
using Microsoft.AspNetCore.Components;

namespace MagicT.WebTemplate;

public partial class App
{
    [Inject]
    private LoginManager LoginManager { get; set; }

    private Func<Task> SignOutFunc { get; set; }


    private bool IsLoaded { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SignOutFunc = SignOutAsync;

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

        StateHasChanged();
    }
}