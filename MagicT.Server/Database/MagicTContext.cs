using MagicT.Shared.Models;
using MagicT.Shared.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Database;

public sealed class MagicTContext : DbContext
{
    public MagicTContext(DbContextOptions<MagicTContext> options) : base(options)
    {
        //Database.EnsureDeleted();
    }

    public DbSet<USERS_BASE> USERS_BASE { get; set; }
    
    public DbSet<USERS> USERS { get; set; }

    public DbSet<AUTHORIZATIONS_BASE> AUTHORIZATIONS_BASE { get; set; }

    public DbSet<ROLES> ROLES { get; set; }
    
    public DbSet<PERMISSIONS> PERMISSIONS { get; set; }
    
    public DbSet<TestModel> TestModel { get; set; }

    public DbSet<USER_ROLES> USER_ROLES { get; set; }

    public DbSet<FAILED_TRANSACTIONS_LOG> FAILED_TRANSACTIONS_LOG { get; set; }

}