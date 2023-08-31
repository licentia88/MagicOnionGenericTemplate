using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Web.Initializers;

public class DataInitializer
{
	public Lazy<List<PERMISSIONS>> PERMISSIONS { get; set; }


	public IRolesMService RolesService { get; set; }

    public IPermissionsService PermissionsService { get; set; }


	public DataInitializer(IServiceProvider provider)
	{
		PERMISSIONS = provider.GetService<Lazy<List<PERMISSIONS>>>();

        PermissionsService = provider.GetService<IPermissionsService>();

		RolesService = provider.GetService<IRolesMService>();
    }

	public async void Initialize()
	{
		var result = await  PermissionsService.Read();

		PERMISSIONS.Value.AddRange(result);
		//PERMISSIONS = new Lazy<List<PERMISSIONS>>(result);
    }

}

