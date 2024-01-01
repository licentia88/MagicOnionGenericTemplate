using System.Text;
using Benutomo;
using MagicT.Shared.Extensions;

namespace MagicT.Server.Managers;

[AutomaticDisposeImpl]
public partial class QueryManager : IDisposable, IAsyncDisposable
{

    public (string query, KeyValuePair<string, object>[] parameters) BuildQuery<TModel>(params KeyValuePair<string, object>[] parameters)
    {
        StringBuilder queryBuilder = new StringBuilder();

        string tableName = typeof(TModel).Name;

        string alias = GenerateRandomAlias();

        queryBuilder.AppendLine($"SELECT * FROM {tableName} {alias}");

        List<Type> parentTypes = GetParentClassNames(typeof(TModel));

        foreach (var parent in parentTypes)
        {
            string parentAlias = GenerateRandomAlias();
            string primaryKey = ModelExtensions.GetPrimaryKey(parent);

            queryBuilder.AppendLine($"LEFT JOIN {parent.Name} {parentAlias} ON {alias}.{primaryKey} = {parentAlias}.{primaryKey}");
        }

        if (parameters is not null && parameters.Any())
        {
            var whereStatement = $" WHERE {string.Join(" AND ", parameters.Select(x => $" {x.Key} = @{x.Key}").ToList())}";

            queryBuilder.Append(whereStatement);
        }

        return (queryBuilder.ToString(), parameters);
    }

    public (string query, KeyValuePair<string, object>[] parameters) BuildQuery<TModel>(byte[] byteParameters = null)
    {
        var  parameters = byteParameters.DeserializeFromBytes<KeyValuePair<string, object>[]>();

        return BuildQuery<TModel>(parameters);
    }


    static string GenerateRandomAlias()
    {
        Random random = new Random();
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numbers = "0123456789";

        StringBuilder randomString = new StringBuilder();

        randomString.Append(letters[random.Next(letters.Length)]);
        randomString.Append(letters[random.Next(letters.Length)]);
        randomString.Append(numbers[random.Next(numbers.Length)]);
        randomString.Append(numbers[random.Next(numbers.Length)]);

        return randomString.ToString();
    }

    static List<Type> GetParentClassNames(Type type)
    {
        List<Type> parentClassNames = new();

        Type currentType = type;

        while (currentType.BaseType != null && currentType.BaseType != typeof(object))
        {
            currentType = currentType.BaseType;

            parentClassNames.Add(currentType);
        }

        return parentClassNames;
    }


}
