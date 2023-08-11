namespace MagicT.Server.Options;

/// <summary>
/// Represents a connection configuration for a database.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class Connections
{
    /// <summary>
    /// Gets or sets the name of the connection.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the connection string for the database.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the database provider name for the connection.
    /// </summary>
    public string Provider { get; set; }
}