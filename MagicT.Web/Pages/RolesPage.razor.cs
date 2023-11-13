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
        await Read(default);
        await base.OnInitializedAsync();
    }

    protected override Task Load(IGenView<ROLES> view)
    {
        if (view.ViewState != ViewState.Create)
            AB_NAME.EditorEnabled = false;

         //Service.WithCancellationToken

        return base.Load(view);


        
    }


}

