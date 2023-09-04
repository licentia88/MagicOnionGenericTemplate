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

    private string label;

    private string displayField;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        label = AUTH_TYPE == nameof(ROLES) ? "Role Name" : "Permission Name";

        displayField = AUTH_TYPE == nameof(ROLES) ? nameof(ROLES.AB_NAME) : nameof(PERMISSIONS.PER_PERMISSION_NAME);

        await ExecuteAsync(async () =>
        {
            var response =  await Service.FindUserRolesByType(AUTH_TYPE);

            DataSource = response;
        });

    }

    protected override async Task Create(GenArgs<USER_ROLES> args)
    {
        //var value = AuthList.Value.FirstOrDefault(x => x.AB_ROWID == args.Model.UR_ROLE_REFNO);

        await base.Create(args);

        //DataSource[args.Index].AUTHORIZATIONS_BASE = value;

        //StateHasChanged();
    }

    protected override async Task Update(GenArgs<USER_ROLES> args)
    {
        //var value = AuthList.Value.FirstOrDefault(x => x.AB_ROWID == args.Model.UR_ROLE_REFNO);

        await base.Update(args);

        //DataSource[args.Index].AUTHORIZATIONS_BASE = value;
     }
}

