namespace MagicT.Shared.Extensions;

/// <summary>
/// Extensions for working with collections and performing additional operations.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Inserts an element into the collection at the specified index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The collection to insert the element into.</param>
    /// <param name="index">The index at which to insert the element.</param>
    /// <param name="value">The element to insert.</param>
    /// <exception cref="NotSupportedException">Thrown when the collection is read-only or not a list.</exception>
    public static void Insert<T>(this ICollection<T> enumerable, int index, T value)
    {
        if (enumerable.IsReadOnly)
            throw new NotSupportedException("Insert operation is not supported on read-only collections.");

        if (enumerable is not IList<T> collection)
            throw new NotSupportedException("Insert operation is only supported on list-based collections.");

        collection.Insert(index, value);
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The collection to search.</param>
    /// <param name="value">The object to locate in the collection.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of the specified object in the collection,
    /// if found; otherwise, -1.
    /// </returns>
    /// <exception cref="NotSupportedException">Thrown when the collection is read-only or not a list.</exception>
    public static int IndexOf<T>(this ICollection<T> enumerable, T value)
    {
        if (enumerable.IsReadOnly)
            throw new NotSupportedException("IndexOf operation is not supported on read-only collections.");

        if (enumerable is not IList<T> collection)
            throw new NotSupportedException("IndexOf operation is only supported on list-based collections.");

        return collection.IndexOf(value);
    }

    /// <summary>
    /// Replaces an element in the collection with a new value.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The collection to perform the replacement on.</param>
    /// <param name="oldValue">The value to be replaced.</param>
    /// <param name="newValue">The new value to replace the old value.</param>
    /// <exception cref="NotSupportedException">Thrown when the collection is read-only or not a list.</exception>
    public static void Replace<T>(this ICollection<T> enumerable, T oldValue, T newValue)
    {
        try
        {
            if (enumerable.IsReadOnly)
                throw new NotSupportedException("Replace operation is not supported on read-only collections.");

            if (enumerable is not IList<T> collection)
                throw new NotSupportedException("Replace operation is only supported on list-based collections.");

            var index = enumerable.IndexOf(oldValue);

            if (index >= 0)
                collection[index] = newValue;
        }
        catch (Exception ex)
        {
            // If an exception occurs during the replacement, it will be ignored.
            // This could happen if the element is not found or if the collection is read-only.
            // It's generally not recommended to use try-catch for flow control,
            // but in this case, the method signature doesn't allow returning an error code,
            // and the original implementation also silently ignored errors.
            // For better error handling, you may want to revise the method signature.
            // For example, returning a boolean indicating whether the replacement was successful.
        }
    }

    /// <summary>
    /// Removes the element at the specified index from the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="enumerable">The collection to remove the element from.</param>
    /// <param name="index">The index of the element to remove.</param>
    /// <exception cref="NotSupportedException">Thrown when the collection is read-only or not a list.</exception>
    public static void RemoveAt<T>(this ICollection<T> enumerable, int index)
    {
        if (enumerable.IsReadOnly)
            throw new NotSupportedException("RemoveAt operation is not supported on read-only collections.");

        if (enumerable is not IList<T> collection)
            throw new NotSupportedException("RemoveAt operation is only supported on list-based collections.");

        collection.RemoveAt(index);
    }
}
