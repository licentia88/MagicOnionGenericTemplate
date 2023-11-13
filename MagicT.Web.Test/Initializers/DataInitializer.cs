using MagicT.Shared.Models.Base;
using MagicT.Shared.Services;

namespace MagicT.Web.Test.Initializers;

public class DataInitializer
{
   public Lazy<List<AUTHORIZATIONS_BASE>> AUTHORIZATIONS_BASE { get; set; }
 
    public IRolesService RolesService { get; set; }

    public IPermissionsService PermissionsService { get; set; }


    public DataInitializer(IServiceProvider provider)
	{
        AUTHORIZATIONS_BASE = provider.GetService<Lazy<List<AUTHORIZATIONS_BASE>>>();


        PermissionsService = provider.GetService<IPermissionsService>();

        RolesService = provider.GetService<IRolesService>();
    }

	public async Task InitializeRolesAsync()
	{
        var roles = await RolesService.ReadAsync();
        var permissions = await PermissionsService.ReadAsync();

        AUTHORIZATIONS_BASE.Value.AddRange(roles);

        AUTHORIZATIONS_BASE.Value.AddRange(permissions);
    }

}

