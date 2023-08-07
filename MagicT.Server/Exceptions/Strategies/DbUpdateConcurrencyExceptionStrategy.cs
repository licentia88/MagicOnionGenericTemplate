using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Exceptions.Strategies;

// Proxy class to handle DbUpdateConcurrencyException
public class DbUpdateConcurrencyExceptionStrategy : IDbExceptionStrategy
{
    public string GetExceptionMessage(Exception exception)
    {
        if (exception is DbUpdateConcurrencyException concurrencyEx)
            return "A concurrency conflict occurred while updating the database: " + concurrencyEx.Message;

        return null;
    }
}