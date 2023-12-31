﻿using Generator.Components.Args;
using Generator.Components.Components;
using Generator.Components.Enums;
using Generator.Components.Interfaces;
using MagicT.Shared.Models;

namespace MagicT.Web.Pages;

public partial class RolesPage
{
    private GenTextField AB_NAME;
 

    protected override async Task OnInitializedAsync()
    {  
        await base.OnInitializedAsync();
    }

    protected override async Task LoadAsync()
    {
        await ReadAsync(default);
        await base.LoadAsync();
    }
    protected override Task ShowAsync(IGenView<ROLES> view)
    {
        if (view.ViewState != ViewState.Create)
            AB_NAME.EditorEnabled = false;

         //Service.WithCancellationToken

        return base.ShowAsync(view);
 
    }

   

    protected override async Task<ROLES> UpdateAsync(GenArgs<ROLES> args)
    {
         return await  base.UpdateAsync(args);
    }

}

