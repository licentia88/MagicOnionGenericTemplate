using Generator.Components.Args;
using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages;

public partial class UserRolesPage
{
	[Inject]
	public Lazy<List<AUTHORIZATIONS_BASE>> AuthList { get; set; }
 
    [Parameter,EditorRequired]
    public string AUTH_TYPE { get; set; }

    private string _label;

    private string _displayField;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _label = AUTH_TYPE == nameof(ROLES) ? "Role Name" : "Permission Name";

        _displayField = AUTH_TYPE == nameof(ROLES) ? nameof(ROLES.AB_NAME) : nameof(PERMISSIONS.PER_PERMISSION_NAME);

        await ExecuteAsync(async () =>
        {
            var response = await Service.FindUserRolesByType(AUTH_TYPE);

            DataSource = response;
        });

    }
}

