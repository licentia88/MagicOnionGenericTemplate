using MagicT.Shared.Models.Base;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Users;

public partial class UserRolesPage
{
    [Inject]
    public Lazy<List<AUTHORIZATIONS_BASE>> AuthList { get; set; }

    [Parameter, EditorRequired]
    public string AUTH_TYPE { get; set; }

    private string _label;

    private string _displayField;



    protected override async Task OnBeforeInitializeAsync()
    {

        _label = AUTH_TYPE == nameof(ROLES) ? "Role Name" : "Permission Name";

        _displayField = AUTH_TYPE == nameof(ROLES) ? nameof(ROLES.AB_NAME) : nameof(PERMISSIONS.PER_PERMISSION_NAME);


        await ExecuteAsync(async () =>
        {
            var response = await Service.FindUserRolesByType(ParentModel.U_ROWID, AUTH_TYPE);

            DataSource = response;
        });
    }
}

