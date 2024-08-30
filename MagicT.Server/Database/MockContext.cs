
namespace MagicT.Server.Database;

/// <summary>
/// Represents a mock database context for testing purposes.
/// </summary>
public class MockContext : MagicTContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MockContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public MockContext(DbContextOptions<MagicTContext> options) : base(options)
    {
    }
}