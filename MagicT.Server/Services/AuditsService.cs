using MagicT.Server.Database;
using MagicT.Server.Services.Base;
using MagicT.Shared.Models;
using MagicT.Shared.Services;

namespace MagicT.Server.Services;

public class AuditsService : MagicServerService<IAuditsService, AUDITS>, IAuditsService
{
    public AuditsService(IServiceProvider provider) : base(provider)
    {
    }
}

