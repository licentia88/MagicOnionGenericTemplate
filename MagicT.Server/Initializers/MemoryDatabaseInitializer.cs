using MagicT.Server.Database;
using MagicT.Shared;
using MagicT.Shared.Models;
using MagicT.Shared.Models.MemoryDatabaseModels;
using MagicT.Shared.Tables;

namespace MagicT.Server.Initializers;

/// <summary>
/// Class responsible for initializing the MagicTContext database and creating a MemoryDatabase from it.
/// </summary>
public sealed class MemoryDatabaseInitializer
{
    private static byte[] _memoryData;

    private readonly MagicTContext _context;
    private readonly DatabaseBuilder _builder;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDatabaseInitializer"/> class.
    /// </summary>
    /// <param name="context">The MagicTContext representing the database context.</param>
    public MemoryDatabaseInitializer(MagicTContext context)
    {
        _context = context;
        _builder = new DatabaseBuilder();
    }

    /// <summary>
    /// Initializes the database and builds the memory data from it.
    /// </summary>
    public void Initialize()
    {
        var test = _context.USERS.ToList();
        // Retrieve the authorizations from the database.
        var authorizations = _context.AUTHORIZATIONS_BASE.ToList();

        // Map and append the authorizations to the memory database builder.
        _builder.Append(authorizations.MapToMemoryModelList<Authorizations>());

        // Map and append the roles to the memory database builder.
        _builder.Append(authorizations.Where(x => x.AB_AUTH_TYPE == nameof(ROLES)).MapToMemoryModelList<Roles>());

        // Map and append the permissions to the memory database builder.
        _builder.Append(authorizations.Where(x => x.AB_AUTH_TYPE == nameof(PERMISSIONS)).MapToMemoryModelList<Permissions>());

        //append users to the memory database builder
        _builder.Append(new List<Users>());
        
        // Build the memory data from the builder.
        _memoryData = _builder.Build();
    }

    /// <summary>
    /// Creates a new instance of the MemoryDatabase from the initialized memory data.
    /// </summary>
    /// <returns>A new MemoryDatabase instance.</returns>
    public static MemoryDatabase CreateMemoryDatabase()
    {
        // Create a new MemoryDatabase instance from the memory data.
        var md = new MemoryDatabase(_memoryData, maxDegreeOfParallelism: Environment.ProcessorCount);

        // Clear the memory data after creating the MemoryDatabase instance.
        _memoryData = null;

        return md;
    }
}