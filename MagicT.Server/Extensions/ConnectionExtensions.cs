using System.Data.Common;
using AQueryMaker;
using AQueryMaker.Interfaces;
using AQueryMaker.MSSql;
using AQueryMaker.Oracle;
using MagicT.Server.Options;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace MagicT.Server.Extensions
{
    /// <summary>
    /// Provides extension methods for handling database connections and creating query factories.
    /// </summary>
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Gets the connection string from the configuration for the specified connection name.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <param name="name">The name of the connection.</param>
        /// <returns>The connection string for the specified connection name.</returns>
        public static string GetConnection(this IConfiguration configuration, string name)
        {
            return configuration.GetSection("ConnectionStrings")
                .Get<List<Connections>>()
                .FirstOrDefault(x => x.Name.Equals(name))?.ConnectionString;
        }

        /// <summary>
        /// Adds the connection factories to the service collection for the specified connections.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="connections">The list of connection settings.</param>
        private static void AddConnectionFactories(this IServiceCollection services, List<Connections> connections)
        {
            services.AddSingleton<IDictionary<string, Func<SqlQueryFactory>>>(provider =>
            {
                var connectionFactories = new Dictionary<string, Func<SqlQueryFactory>>();

                foreach (var connectionSetting in connections)
                {
                    connectionFactories.Add(connectionSetting.Name,
                        () => new SqlQueryFactory(AddDatabaseResolver(connectionSetting)));
                }

                return connectionFactories;
            });
        }

        /// <summary>
        /// Gets the list of connection strings from the configuration.
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        /// <returns>The list of connection settings.</returns>
        internal static List<Connections> GetConnectionStrings(IConfiguration configuration)
        {
            return configuration.GetSection("ConnectionStrings").Get<List<Connections>>();
        }

        /// <summary>
        /// Creates a configuration object from the appsettings.json file.
        /// </summary>
        /// <returns>The configuration object.</returns>
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }

        /// <summary>
        /// Creates a database connection using the specified connection settings.
        /// </summary>
        /// <param name="settings">The connection settings.</param>
        /// <returns>The database connection.</returns>
        private static DbConnection CreateConnection(Connections settings)
        {
            var factory = DbProviderFactories.GetFactory(settings.Provider);
            var connection = factory.CreateConnection();
            if (connection == null) return null;
            connection.ConnectionString = settings.ConnectionString;
            return connection;
        }

        /// <summary>
        /// Adds a database manager for the specified connection settings.
        /// </summary>
        /// <param name="settings">The connection settings.</param>
        /// <returns>The database manager.</returns>
        private static IDatabaseManager AddDatabaseResolver(Connections settings)
        {
            var connection = CreateConnection(settings);

            if (connection is OracleConnection)
                return new OracleServerManager(connection);

            if (connection is SqlConnection)
                return new SqlServerManager(connection);

            throw new InvalidOperationException("Unsupported database type");
        }
    }
}
