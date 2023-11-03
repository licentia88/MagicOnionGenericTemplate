using Generator.Components.Components;
using Generator.Components.Enums;
using Generator.Components.Interfaces;
using MagicT.Shared.Models;

namespace MagicT.Web.Pages;

public partial class RolesPermissions
{
    public GenTextField PER_PERMISSION_NAME { get; set; }
    //[Inject]
    //public Lazy<List<PERMISSIONS>> PermissionsList { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await FindByParent();
        await base.OnInitializedAsync();
    }

    protected override Task Load(IGenView<PERMISSIONS> view)
    {
        if (view.ViewState != ViewState.Create)
            PER_PERMISSION_NAME.EditorVisible = false;

        return base.Load(view);
    }

}

