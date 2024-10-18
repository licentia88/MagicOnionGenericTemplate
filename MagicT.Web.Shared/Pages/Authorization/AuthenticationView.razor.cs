﻿using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Shared.Pages.Authorization;

public partial class AuthenticationView
{
    [Inject]
    public NavigationManager NavigationManager { get; set; }


    [CascadingParameter(Name = nameof(LoginData))]
    public AuthenticationRequest LoginData { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public RenderFragment Authenticated { get; set; }

    [Parameter]
    public RenderFragment NotAuthenticated { get; set; }


}

