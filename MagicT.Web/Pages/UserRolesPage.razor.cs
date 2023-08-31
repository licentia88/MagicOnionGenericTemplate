using System;
using MagicT.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages;

public partial class UserRolesPage
{
	[Inject]
	public Lazy<List<PERMISSIONS>> Permissions { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await FindByParent();
        await base.OnInitializedAsync();
    }
}

