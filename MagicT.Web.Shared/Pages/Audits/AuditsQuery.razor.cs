using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Audits;

public partial class AuditsQuery
{
    [Inject]
    public Lazy<List<USERS>> UsersList { get; set; }


    [Inject]
    public Lazy<List<Operations>> OperationsList { get; set; }

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }
}

