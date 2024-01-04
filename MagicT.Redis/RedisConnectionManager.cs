using Benutomo;
using MagicT.Redis.Options;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MagicT.Redis;

[AutomaticDisposeImpl]
public partial class RedisConnectionManager:IDisposable,IAsyncDisposable
{
    private readonly MagicTRedisConfig MagicTRedisConfig;

    [EnableAutomaticDispose]
    public ConnectionMultiplexer ConnectionMultiplexer;

    public RedisConnectionManager(IConfiguration configuration)
    {
        MagicTRedisConfig = configuration.GetSection("MagicTRedisConfig").Get<MagicTRedisConfig>();
 
        ConnectionMultiplexer = CreateConnectionMultiplexer();
    }

    private ConnectionMultiplexer CreateConnectionMultiplexer()
    {
        return ConnectionMultiplexer.Connect(MagicTRedisConfig.ConnectionString);
        // Read the Redis connection string from the configuration section "MagicTRedisDatabase"
    }
}