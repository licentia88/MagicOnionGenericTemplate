using System.Data.Common;
using AQueryMaker;
using AQueryMaker.Interfaces;
using AQueryMaker.MSSql;
using AQueryMaker.Oracle;
using MagicT.Server.Options;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace MagicT.Server.Extensions;

public static class ConnectionExtensions
{
    public static string GetConnection(this IConfiguration configuration, string Name)
    {
        return configuration.GetSection("ConnectionStrings").Get<List<Connections>>().FirstOrDefault(x => x.Name.Equals(Name))?.ConnectionString;
    }
 
    private static void AddConnectionFactories(this IServiceCollection services, List<Connections> connections)
    {
        services.AddSingleton<IDictionary<string, Func<SqlQueryFactory>>>(provider =>
        {
            var connectionFactories = new Dictionary<string, Func<SqlQueryFactory>>();

            foreach (var ConnectionSetting in connections)
            {
                connectionFactories.Add(ConnectionSetting.Name, () => new SqlQueryFactory(AddDatabaseResolver(ConnectionSetting)));
            }
            return connectionFactories;
        });
    }

    internal static List<Connections> GetConnectionStrings(IConfiguration configuration)
    {
        return configuration.GetSection("ConnectionStrings").Get<List<Connections>>();

    }

    public static IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    private static DbConnection CreateConnection(Connections settings)
    {
        var factory = DbProviderFactories.GetFactory(settings.Provider);
        var connection = factory.CreateConnection();
        if (connection == null) return null;
        connection.ConnectionString = settings.ConnectionString;
        return connection;
    }


    internal static IDatabaseManager AddDatabaseResolver(Connections settings)
    {
        var connection = CreateConnection(settings);

        if (connection is OracleConnection)
        {
            return new OracleServerManager(connection);
        }

        if (connection is SqlConnection)
        {
            return new SqlServerManager(connection);
        }


        throw new InvalidOperationException("Unsupported database type");



    }

}
