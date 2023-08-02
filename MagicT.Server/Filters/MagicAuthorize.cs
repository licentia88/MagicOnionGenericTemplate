using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using MagicT.Server.Jwt;

namespace MagicT.Server.Filters;

public class MagicAuthorize : Attribute, IMagicOnionFilterFactory<IMagicOnionServiceFilter>, IMagicOnionServiceFilter
{
    private int[] _Roles { get; }

    public IServiceProvider _serviceProvider { get; set; }

    //[Inject]
    //public List<USER_AUTHORIZATIONS> UserAuthorizationsList { get; set; }

    public MagicAuthorize(params int[] Roles)
    {
        _Roles = Roles;
    }


    public IMagicOnionServiceFilter CreateInstance(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        //UserAuthorizationsList = serviceProvider.GetService<List<USER_AUTHORIZATIONS>>();

        //serviceProvider.GetRequiredService<IRoleService>().GetRoles();

        return this;
    }

    public async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        var isAllowed = context.AttributeLookup.Any((IGrouping<Type, Attribute> arg) => arg.Key == typeof(AllowAttribute));

        if (isAllowed)
        {
            await next(context);
            return;
        }

        var tokenResult = ProcessToken(context);

        await next(context);
    }

    private bool ProcessToken(ServiceContext context)
    {
        using var scope = _serviceProvider.CreateScope();
        var fastJwtTokenService = scope.ServiceProvider.GetRequiredService<MagicTTokenService>();

        var tokenHeader = context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == "auth-token-bin");

        if (tokenHeader is null)
            throw new ReturnStatusException(StatusCode.PermissionDenied, "Security Token not found");


        return fastJwtTokenService.DecodeToken(tokenHeader.ValueBytes, _Roles);
    }

}

