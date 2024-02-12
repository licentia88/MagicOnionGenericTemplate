using Generator.Components.Args;
using Generator.Components.Components;
using Generator.Components.Enums;
using Generator.Components.Interfaces;
using MagicT.Shared.Models;

namespace MagicT.WebTemplate.Pages;

public partial class RolesPage
{
    private GenTextField AB_NAME;
 

    protected override async Task OnInitializedAsync()
    {  
        await base.OnInitializedAsync();
    }

    protected override async Task OnBeforeInitializeAsync()
    {
        await ReadAsync(default);
        await base.OnBeforeInitializeAsync();
    }
    protected override Task LoadAsync(IGenView<ROLES> view)
    {
        if (view.ViewState != ViewState.Create)
            AB_NAME.Disabled = true;

         //Service.WithCancellationToken

        return base.LoadAsync(view);
 
    }

   

    protected override async Task<ROLES> UpdateAsync(GenArgs<ROLES> args)
    {
         return await  base.UpdateAsync(args);
    }

}

