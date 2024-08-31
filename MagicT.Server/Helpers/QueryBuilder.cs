﻿using System.Text;
using Benutomo;
using MagicT.Shared.Extensions;
using ModelExtensions = MagicT.Shared.Extensions.ModelExtensions;

namespace MagicT.Server.Helpers;

/// <summary>
/// Provides methods to build SQL queries for a given model.
/// </summary>
[AutomaticDisposeImpl]
public partial class QueryBuilder : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Builds a SQL query for the specified model type with the given parameters.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="parameters">The parameters to include in the query.</param>
    /// <returns>A tuple containing the query string and the parameters.</returns>
    public static (string query, KeyValuePair<string, object>[] parameters) BuildQuery<TModel>(params KeyValuePair<string, object>[] parameters)
    {
        var queryBuilder = new StringBuilder();
        var tableName = typeof(TModel).Name;
        var alias = GenerateRandomAlias();

        queryBuilder.AppendLine($"SELECT * FROM {tableName} {alias}");

        foreach (var parent in GetParentClassNames(typeof(TModel)))
        {
            var parentAlias = GenerateRandomAlias();
            var primaryKey = ModelExtensions.GetPrimaryKey(parent);
            queryBuilder.AppendLine($"LEFT JOIN {parent.Name} {parentAlias} ON {alias}.{primaryKey} = {parentAlias}.{primaryKey}");
        }

        if (parameters?.Any() == true)
        {
            var whereStatement = $" WHERE {string.Join(" AND ", parameters.Select(x => $" {x.Key} = @{x.Key}"))}";
            queryBuilder.Append(whereStatement);
        }

        return (queryBuilder.ToString(), parameters);
    }

    /// <summary>
    /// Builds a SQL query for the specified model type with the given byte array parameters.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="byteParameters">The byte array containing the parameters.</param>
    /// <returns>A tuple containing the query string and the parameters.</returns>
    public static (string query, KeyValuePair<string, object>[] parameters) BuildQuery<TModel>(byte[] byteParameters = null)
    {
        var parameters = byteParameters?.DeserializeFromBytes<KeyValuePair<string, object>[]>();
        return BuildQuery<TModel>(parameters);
    }

    /// <summary>
    /// Generates a random alias for use in SQL queries.
    /// </summary>
    /// <returns>A random alias string.</returns>
    private static string GenerateRandomAlias()
    {
        var random = new Random();
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";

        return $"{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{numbers[random.Next(numbers.Length)]}{numbers[random.Next(numbers.Length)]}";
    }

    /// <summary>
    /// Gets the parent class names for the specified type.
    /// </summary>
    /// <param name="type">The type to get the parent class names for.</param>
    /// <returns>A list of parent class types.</returns>
    private static List<Type> GetParentClassNames(Type type)
    {
        var parentClassNames = new List<Type>();
        var currentType = type;

        while (currentType.BaseType != null && currentType.BaseType != typeof(object))
        {
            currentType = currentType.BaseType;
            parentClassNames.Add(currentType);
        }

        return parentClassNames;
    }
}