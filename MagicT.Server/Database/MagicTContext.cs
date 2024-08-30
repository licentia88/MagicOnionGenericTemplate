using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace MagicT.Server.Database;

/// <summary>
/// Represents the database context for the MagicT application.
/// </summary>
public class MagicTContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MagicTContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public MagicTContext(DbContextOptions<MagicTContext> options) : base(options)
    {
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
        // Database.Migrate();
    }

    /// <summary>
    /// Gets or sets the DbSet for users.
    /// </summary>
    public DbSet<USERS> USERS { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for authorizations base.
    /// </summary>
    public DbSet<AUTHORIZATIONS_BASE> AUTHORIZATIONS_BASE { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for roles.
    /// </summary>
    public DbSet<ROLES> ROLES { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for permissions.
    /// </summary>
    public DbSet<PERMISSIONS> PERMISSIONS { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for test models.
    /// </summary>
    public DbSet<TestModel> TestModel { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for user roles.
    /// </summary>
    public DbSet<USER_ROLES> USER_ROLES { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for failed transactions.
    /// </summary>
    public DbSet<AUDIT_FAILED> FAILED_TRANSACTIONS { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for audit base.
    /// </summary>
    public DbSet<AUDIT_BASE> AUDIT_BASE { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for audit queries.
    /// </summary>
    public DbSet<AUDIT_QUERY> AUDIT_QUERY { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for audit records.
    /// </summary>
    public DbSet<AUDIT_RECORDS> AUDIT_RECORDS { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for audit records details.
    /// </summary>
    public DbSet<AUDIT_RECORDS_D> AUDIT_RECORDS_D { get; set; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
    {
        var result = base.SaveChanges();
        ChangeTracker.Clear();
        return result;
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        ChangeTracker.Clear();
        return result;
    }
}