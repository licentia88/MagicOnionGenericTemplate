using MagicT.Server.Exceptions.Strategies;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Exceptions;

/// <summary>
/// A class that handles exceptions specific to database operations.
/// </summary>
public sealed class DbExceptionHandler
{
    private readonly Dictionary<Type, IDbExceptionStrategy> exceptionStrategies;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DbExceptionHandler" /> class.
    /// </summary>
    public DbExceptionHandler()
    {
        exceptionStrategies = new Dictionary<Type, IDbExceptionStrategy>
        {
            {typeof(DbUpdateConcurrencyException), new DbUpdateConcurrencyExceptionStrategy()},
            {typeof(DbUpdateException), new DbUpdateExceptionStrategy()},
            {typeof(InvalidOperationException), new InvalidOperationExceptionStrategy()},
            {typeof(NotSupportedException), new NotSupportedExceptionStrategy()},
            {typeof(ObjectDisposedException), new ObjectDisposedExceptionStrategy()}
        };
    }

    /// <summary>
    ///     Handles the specified exception and returns an error message.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    /// <returns>An error message related to the exception.</returns>
    public string HandleException(Exception ex)
    {
        if (exceptionStrategies.TryGetValue(ex.GetType(), out var strategy))
            return strategy.GetExceptionMessage(ex);
        // Handle the exception in a generic way or return an appropriate error message
        return "An error occurred.";
    }
}