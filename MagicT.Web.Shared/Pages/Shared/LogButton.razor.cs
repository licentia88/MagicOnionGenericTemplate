using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Shared;

public partial class LogButton
{
    [Parameter,EditorRequired]
    public EventCallback OnClick { get; set; }
}