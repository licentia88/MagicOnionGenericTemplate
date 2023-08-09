namespace MagicT.Client.Exceptions;

/// <summary>
/// Represents an exception related to client filtering.
/// </summary>
public class FilterException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilterException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public FilterException(string message) : base(message)
    {
    }
}
