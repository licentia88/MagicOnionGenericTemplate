using MagicT.Shared.Models.ServiceModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Test.Shared.AuthorizationViews;

public partial class AuthenticationView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }


    [CascadingParameter(Name =nameof(LoginData))] 
    public (string Identifier, EncryptedData<string> SecurePassword) LoginData { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public RenderFragment Authenticated { get; set; }

    [Parameter]
    public RenderFragment NotAuthenticated { get; set; }

     
}

