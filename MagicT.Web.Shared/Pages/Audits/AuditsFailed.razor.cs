using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Audits;

public partial class AuditsFailed
{
    [Inject]
    public Lazy<List<USERS>> UsersList { get; set; }

    [Inject]
    public Lazy<List<Operations>> OperationsList { get; set; }
}

