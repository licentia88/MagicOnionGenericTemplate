﻿using MagicT.Shared.Models;
using MagicT.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MagicT.Web.Pages.Audits;

public partial class AuditsQuery
{
    [Inject]
    public Lazy<List<USERS>> UsersList { get; set; }


    [Inject]
    public Lazy<List<Operations>> OperationsList { get; set; }

   
}

