namespace MagicT.Shared.Extensions;

public static class CollectionExtensions
{

    public static void Insert<T>(this ICollection<T> enumerable, int index, T value)
    {
        if (enumerable.IsReadOnly) throw new NotSupportedException();

        if (enumerable is not IList<T> collection) throw new NotSupportedException();

        collection.Insert(index, value);
    }

    public static int IndexOf<T>(this ICollection<T> enumerable, T value)
    {
        if (enumerable.IsReadOnly) throw new NotSupportedException();

        if (enumerable is not IList<T> collection) throw new NotSupportedException();

        return collection.IndexOf(value);
    }

    public static void Replace<T>(this ICollection<T> enumerable, T oldValue, T newValue)
    {
        try
        {
            if (enumerable.IsReadOnly) throw new NotSupportedException();

            if (enumerable is not IList<T> collection) throw new NotSupportedException();

            var index = enumerable.IndexOf(oldValue);

            collection[index] = newValue;
        }
        catch (Exception ex)
        {
            // ignored
        }
    }

    public static void RemoveAt<T>(this ICollection<T> enumerable, int index)
    {
        if (enumerable.IsReadOnly) throw new NotSupportedException();

        if (enumerable is not IList<T> collection) throw new NotSupportedException();

        collection.RemoveAt(index);

    }
}
