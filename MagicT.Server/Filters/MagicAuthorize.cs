using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using MagicT.Server.Jwt;

namespace MagicT.Server.Filters;

public class MagicAuthorize : Attribute, IMagicOnionFilterFactory<IMagicOnionServiceFilter>, IMagicOnionServiceFilter
{
    private int[] Roles { get; }

    private IServiceProvider ServiceProvider { get; set; }

    //[Inject]
    //public List<USER_AUTHORIZATIONS> UserAuthorizationsList { get; set; }

    public MagicAuthorize(params int[] Roles)
    {
        this.Roles = Roles;
    }


    public IMagicOnionServiceFilter CreateInstance(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        //UserAuthorizationsList = serviceProvider.GetService<List<USER_AUTHORIZATIONS>>();

        //serviceProvider.GetRequiredService<IRoleService>().GetRoles();

        return this;
    }

    public async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        var isAllowed = context.AttributeLookup.Any(arg => arg.Key == typeof(AllowAttribute));

        if (isAllowed)
        {
            await next(context);
            return;
        }

        var tokenResult = ProcessToken(context);

        await next(context);
    }

    private bool ProcessToken(IServiceContext context)
    {
        using var scope = ServiceProvider.CreateScope();
        var fastJwtTokenService = scope.ServiceProvider.GetRequiredService<MagicTTokenService>();

        var tokenHeader = context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == "auth-token-bin");

        if (tokenHeader is null)
            throw new ReturnStatusException(StatusCode.PermissionDenied, "Security Token not found");


        return fastJwtTokenService.DecodeToken(tokenHeader.ValueBytes, Roles);
    }

}

