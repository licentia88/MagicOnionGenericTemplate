using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Database;

public class MockContext : MagicTContext
{
    public MockContext(DbContextOptions<MagicTContext> options) : base(options)
    {
    }
    
}
