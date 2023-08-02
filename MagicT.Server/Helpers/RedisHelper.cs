using StackExchange.Redis;

public class RedisHelper
{
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection;

    static RedisHelper()
    {
        string connectionString = "localhost:6379";
        LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
    }

    public static ConnectionMultiplexer Connection => LazyConnection.Value;
}


