using MagicT.Server.Database;
using MagicT.Shared;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models;
using MagicT.Shared.Models.MemoryDatabaseModels;

namespace MagicT.Server.Initializers;

public class DbInitializer
{
    private static byte[] memoryData;

    private readonly MagicTContext _context;
    private readonly DatabaseBuilder _builder;

    public DbInitializer(MagicTContext context)
    {

        _context = context;
        _builder = new DatabaseBuilder();
    }

    public void Initialize()
    {
        var authorizations = _context.AUTHORIZATIONS_BASE.ToList();

        _builder.Append(authorizations.MapToMemoryModelList<Authorizations>());

        _builder.Append(authorizations.Where(x=> x.AB_AUTH_TYPE == nameof(ROLES)).MapToMemoryModelList<Roles>());

        _builder.Append(authorizations.Where(x => x.AB_AUTH_TYPE == nameof(PERMISSIONS )).MapToMemoryModelList<Permissions>());


        memoryData = _builder.Build();
    }

    public static MemoryDatabase CreateMemoryDatabase()
    {
        var md = new MemoryDatabase(memoryData, maxDegreeOfParallelism: Environment.ProcessorCount);

        memoryData = null;

        return md;
    }
}

