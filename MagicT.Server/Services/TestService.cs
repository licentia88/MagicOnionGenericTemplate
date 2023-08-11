﻿using MagicT.Server.Database;
using MagicT.Server.Filters;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

[MagicTAuthorize(1)]
// ReSharper disable once UnusedType.Global
public sealed class TestService : MagicTServerServiceBase<ITestService, TestModel, MagicTContext>, ITestService
{
    public TestService(IServiceProvider provider) : base(provider)
    {
    }
}