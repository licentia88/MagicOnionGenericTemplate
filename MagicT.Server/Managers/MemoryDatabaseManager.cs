using MagicT.Server.Database;
using MagicT.Shared;
using MagicT.Shared.Models;
using MagicT.Shared.Models.MemoryDatabaseModels;

/// <summary>
/// Manages an in-memory database for authorization data.
/// </summary>
public class MemoryDatabaseManager
{
    /// <summary>
    /// The read-only MemoryDatabase instance.
    /// </summary> 
    public static MemoryDatabase MemoryDatabase { get; private set; }

    /// <summary>
    /// Gets a read-write builder for modifying the database.
    /// Initializes the builder if it is null.
    /// </summary>
    public static ImmutableBuilder MemoryDatabaseRW()
    {
        if(immutableBuilder is null)
            immutableBuilder = MemoryDatabase.ToImmutableBuilder();

        return immutableBuilder;
    }


    private static ImmutableBuilder immutableBuilder;

    /// <summary>
    /// Database context for querying persistent storage.
    /// </summary>
    private readonly MagicTContext _context;

    /// <summary>
    /// Builder for constructing the in-memory database.
    /// </summary>
    private readonly DatabaseBuilder _builder;
 
    /// <summary>
    /// Constructs a new MemoryDatabaseManager.
    /// </summary>
    /// <param name="provider">Service provider.</param>
    public MemoryDatabaseManager(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<MagicTContext>();
        _builder = new DatabaseBuilder();
        // Query persistent storage and build in-memory database.

        var authorizations = _context.AUTHORIZATIONS_BASE.ToList();

        // Map and append the authorizations.
        _builder.Append(authorizations.MapToMemoryModelList<Authorizations>());

        // Map and append the roles.
        _builder.Append(authorizations.Where(x => x.AB_AUTH_TYPE == nameof(ROLES))
                     .MapToMemoryModelList<Roles>());

        // Map and append the permissions. 
        _builder.Append(authorizations.Where(x => x.AB_AUTH_TYPE == nameof(PERMISSIONS))
                      .MapToMemoryModelList<Permissions>());

        // Append empty users list.
        _builder.Append(new List<Users>());

        // Build the in-memory binary data.
        var memoryData = _builder.Build();

        // Create MemoryDatabase instance.
        MemoryDatabase = new MemoryDatabase(memoryData);
    }

    /// <summary>
    /// Creates a new empty MemoryDatabase instance.
    /// </summary>
    /// <returns>The new MemoryDatabase instance.</returns>
    public MemoryDatabase CreateNewDatabase()
    {
        // Build new binary data.
        byte[] data = _builder.Build();

        // Create new MemoryDatabase.
        return new MemoryDatabase(data);
    }

    /// <summary> 
    /// Updates the read-only database instance from the read-write builder.
    /// </summary>
    public void UpdateDatabase()
    {
        MemoryDatabase = MemoryDatabaseRW().Build();
    }
}

