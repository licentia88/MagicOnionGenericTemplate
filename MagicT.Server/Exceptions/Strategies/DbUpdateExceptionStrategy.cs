using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Exceptions.Strategies;

// Proxy class to handle DbUpdateException
public class DbUpdateExceptionStrategy : IDbExceptionStrategy
{
    public string GetExceptionMessage(Exception exception)
    {
        if (exception is DbUpdateException dbUpdateEx)
        {
            if (dbUpdateEx.InnerException is SqlException sqlEx)
                switch (sqlEx.Number)
                {
                    case 547: // Foreign key constraint violation
                        return "The record is associated with other data and does not have a valid foreignkey";
                    case 2627: // Primary key violation
                        return "The record already exists.";
                    case 515: // NULL value violation
                        return "Some required fields were not provided.";
                    case 2601: // Unique key constraint violation
                        return "Duplicate values are not allowed.";
                    case -2: // Timeout expired
                        return "The operation timed out. Please try again later.";
                    case 3621: // Invalid object name
                        return "The specified table or object does not exist.";
                    case 8152: // String or binary data truncated
                        return "The provided data exceeds the allowed length.";
                    case 5471: // CHECK constraint violation
                        return "The provided value violates a data constraint.";
                }

            return "An error occurred while updating the database: ";
        }

        return null;
    }
}