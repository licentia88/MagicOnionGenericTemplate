using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using MagicT.Shared.Services;

namespace MagicT.Web.Initializers;

public class DataInitializer
{
    public Lazy<List<AUTHORIZATIONS_BASE>> AUTHORIZATIONS_BASE { get; set; }

    public Lazy<List<USERS>> USERS { get; set; }

    public IInitializerService InitializerService { get; set; }
 
    public DataInitializer(IServiceProvider provider)
	{
        AUTHORIZATIONS_BASE = provider.GetService<Lazy<List<AUTHORIZATIONS_BASE>>>();

        USERS = provider.GetService<Lazy<List<USERS>>>();

        InitializerService = provider.GetService<IInitializerService>();

       
    }

	public async Task Initialize()
	{

        var users = await InitializerService.GetUsers();

        var roles = await InitializerService.GetRoles();

        var permissions = await InitializerService.GetPermissions();


        USERS.Value.AddRange(users);

        AUTHORIZATIONS_BASE.Value.AddRange(roles);

        AUTHORIZATIONS_BASE.Value.AddRange(permissions);
    }

}

