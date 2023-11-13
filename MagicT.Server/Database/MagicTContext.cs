using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Database;

public class MagicTContext: DbContext
{
    public MagicTContext(DbContextOptions<MagicTContext> options) : base(options)
    {
        //Database.EnsureDeleted();
    }

    public DbSet<USERS_BASE> USERS_BASE { get; set; }
    
    public DbSet<USERS> USERS { get; set; }

    public DbSet<SUPER_USER> SUPER_USER { get; set; }
 
    public DbSet<AUTHORIZATIONS_BASE> AUTHORIZATIONS_BASE { get; set; }

    public DbSet<ROLES> ROLES { get; set; }
    
    public DbSet<PERMISSIONS> PERMISSIONS { get; set; }
    
    public DbSet<TestModel> TestModel { get; set; }

    public DbSet<USER_ROLES> USER_ROLES { get; set; }

    public DbSet<AUDIT_FAILED> FAILED_TRANSACTIONS { get; set; }

    public DbSet<AUDIT_BASE> AUDIT_BASE { get; set; }
    
    public DbSet<AUDIT_QUERY> AUDIT_QUERY { get; set; }
    
    public DbSet<AUDIT_RECORDS> AUDIT_RECORDS { get; set; }
}