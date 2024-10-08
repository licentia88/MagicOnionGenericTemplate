using Generator.Components.Enums;
using Generator.Components.Interfaces;

namespace MagicT.Web.Shared.Pages.Users;

public partial class RolesPermissions
{
    private GenTextField PER_PERMISSION_NAME { get; set; }
    //[Inject]
    //public Lazy<List<PERMISSIONS>> PermissionsList { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await FindByParentEncryptedAsync();
        // await FindByParentAsync();
        await base.OnInitializedAsync();
    }

    protected override Task LoadAsync(IGenView<PERMISSIONS> view)
    {
        if (view.ViewState != ViewState.Create)
            PER_PERMISSION_NAME.EditorVisible = false;

        return base.LoadAsync(view);
    }

}

