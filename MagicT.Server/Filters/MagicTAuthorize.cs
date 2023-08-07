using Grpc.Core;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Filters;
using MagicT.Server.Jwt;

namespace MagicT.Server.Filters;

public class MagicTAuthorize : Attribute, IMagicOnionFilterFactory<IMagicOnionServiceFilter>, IMagicOnionServiceFilter
{
    private int[] Roles { get; }

    private IServiceProvider ServiceProvider { get; set; }

    public MagicTTokenService MagicTTokenService { get; set; }

    //[Inject]
    //public List<USER_AUTHORIZATIONS> UserAuthorizationsList { get; set; }

    public MagicTAuthorize(params int[] Roles)
    {
        this.Roles = Roles;
    }

   

    public IMagicOnionServiceFilter CreateInstance(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;

        MagicTTokenService = ServiceProvider.GetRequiredService<MagicTTokenService>();

        //UserAuthorizationsList = serviceProvider.GetService<List<USER_AUTHORIZATIONS>>();

        //serviceProvider.GetRequiredService<IRoleService>().GetRoles();

        return this;
    }

    public async ValueTask Invoke(ServiceContext context, Func<ServiceContext, ValueTask> next)
    {
        var isAllowed = context.AttributeLookup.Any(arg => arg.Key == typeof(AllowAttribute));

        if (!isAllowed)
        {
            var token = ProcessToken(context);

            ValidateRoles(token, Roles);
        }

        await next(context);
       
    }

    private MagicTToken ProcessToken(IServiceContext context)
    {
        var tokenHeader = context.CallContext.RequestHeaders.FirstOrDefault(x => x.Key == "auth-token-bin");

        if (tokenHeader is null)
            throw new ReturnStatusException(StatusCode.NotFound, "Security Token not found");

        return MagicTTokenService.DecodeToken(tokenHeader.ValueBytes);
    }

    public void ValidateRoles(MagicTToken token, params int[] requiredRoles)
    {
        if (!token.Roles.Any(requiredRoles.Contains)) throw new ReturnStatusException(StatusCode.Unauthenticated, nameof(StatusCode.Unauthenticated));
    }
}