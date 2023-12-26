using Grpc.Core;
using MagicOnion;
using MagicT.Client.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Client.Services;

public sealed class InitializerService : MagicClientServiceBase<IInitializerService>, IInitializerService
{
    public InitializerService(IServiceProvider provider) : base(provider)
    {
    }

    public UnaryResult<List<PERMISSIONS>> GetPermissions()
    {
        return Client.GetPermissions();
    }

    public UnaryResult<List<ROLES>> GetRoles()
    {
        return Client.GetRoles();
    }

    public UnaryResult<List<USERS>> GetUsers()
    {
        return Client.GetUsers();
    }

    public IInitializerService WithCancellationToken(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IInitializerService WithDeadline(DateTime deadline)
    {
        throw new NotImplementedException();
    }

    public IInitializerService WithHeaders(Metadata headers)
    {
        throw new NotImplementedException();
    }

    public IInitializerService WithHost(string host)
    {
        throw new NotImplementedException();
    }

    public IInitializerService WithOptions(CallOptions option)
    {
        throw new NotImplementedException();
    }
}

