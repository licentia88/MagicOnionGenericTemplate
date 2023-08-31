using System;
using DocumentFormat.OpenXml.EMMA;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models;
using MagicT.Web.Extensions;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages;

public partial class RolesDPage
{
    [Inject]
    public Lazy<List<PERMISSIONS>> PermissionsList { get; set; }
    protected override async Task OnInitializedAsync()
    {
        await FindByParent();
        await base.OnInitializedAsync();
    }
 
}

