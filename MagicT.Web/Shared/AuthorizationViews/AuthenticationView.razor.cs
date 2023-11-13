using MagicT.Shared.Models.ServiceModels;
using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.AuthorizationViews;

public partial class AuthenticationView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }


    [CascadingParameter(Name =nameof(LoginData))] 
    public LoginRequest LoginData { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public RenderFragment Authenticated { get; set; }

    [Parameter]
    public RenderFragment NotAuthenticated { get; set; }

     
}

