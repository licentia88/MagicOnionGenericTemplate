using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Database;

public class MagicTContext: DbContext
{
    
    public MagicTContext(DbContextOptions<MagicTContext> options) : base(options)
    {
        // Database.EnsureDeleted();

        //Database.EnsureCreated();
        //Database.Migrate();
    }

   
    public DbSet<USERS> USERS { get; set; }

  
    public DbSet<AUTHORIZATIONS_BASE> AUTHORIZATIONS_BASE { get; set; }

    public DbSet<ROLES> ROLES { get; set; }
    
    public DbSet<PERMISSIONS> PERMISSIONS { get; set; }
    
    public DbSet<TestModel> TestModel { get; set; }

    public DbSet<USER_ROLES> USER_ROLES { get; set; }

    public DbSet<AUDIT_FAILED> FAILED_TRANSACTIONS { get; set; }

    public DbSet<AUDIT_BASE> AUDIT_BASE { get; set; }

    public DbSet<AUDIT_QUERY> AUDIT_QUERY { get; set; }

    public DbSet<AUDIT_RECORDS> AUDIT_RECORDS { get; set; }

    public DbSet<AUDIT_RECORDS_D> AUDIT_RECORDS_D { get; set; }

    public override int SaveChanges()
    {
        var result = base.SaveChanges();
        ChangeTracker.Clear();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        ChangeTracker.Clear();
        return result;
    }
}