﻿using MagicT.Shared.Services.Base;

namespace MagicT.Web.Pages.Base;

public abstract class SecuredServicePageBase<TModel, TService> : ServicePageBase<TModel,TService>
    where TModel : new()
    where TService : IMagicTService<TService, TModel>
{
    public new ISecuredMagicTService<TService, TModel> Service => base.Service as ISecuredMagicTService<TService, TModel>;
}