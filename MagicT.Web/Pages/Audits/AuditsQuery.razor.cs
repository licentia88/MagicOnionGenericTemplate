using MagicT.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages.Audits;

public partial class AuditsQuery
{
    [Inject]
    public Lazy<List<USERS>> UsersList { get; set; }
}

