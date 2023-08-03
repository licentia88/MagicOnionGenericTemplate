namespace MagicT.Server.Options;

internal abstract class Connections
{
    public string Name { get; set; }

    public string ConnectionString { get; set; }

    public string Provider { get; set; }
}

