using MagicT.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Database;

public class CredentialsContext : DbContext
{
    public CredentialsContext(DbContextOptions<CredentialsContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
  
    public DbSet<UsedTokens> UsedTokens { get; set; }

    public DbSet<UsersCredentials> UsersCredentials { get; set; }
}
