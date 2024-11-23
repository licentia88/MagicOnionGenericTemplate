using Benutomo;
using MagicOnion;
using Serilog;

namespace MagicT.Shared.Managers;

/// <summary>
/// Manages logging operations for a specified service.
/// </summary>
/// <typeparam name="TService">The type of the service.</typeparam>
// [RegisterSingleton(typeof(IDisposable))]
[AutomaticDisposeImpl]
public partial class LogManager<TService>: IDisposable
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogManager{TService}"/> class.
    /// </summary>
    public LogManager()
    {
        _logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Async(a => a.Console()) // Async logging to Console
            .WriteTo.Async(a => a.Map("ServiceName", null, (serviceName, writer) =>
            {
                var filename = serviceName ?? "Default";
                writer.File($".Logs/{filename}.Log");
            })) // Async logging to files based on ServiceName
            .CreateLogger()
            .ForContext("ServiceName", typeof(TService).Name);
    }

    ~LogManager()
    {
        Dispose(false);
    }
    
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The member name of the caller.</param>
    public void LogMessage(int userId, string message, string callerFilePath = default, string callerMemberName = default)
    {
        _logger.Information("User:{UserId} Message:{Message} Path:{CallerFilePath} Method:{CallerMemberName}", userId, message, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="message">The error message to log.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The member name of the caller.</param>
    /// <param name="callerLineNumber">The line number in the source file where the error occurred.</param>
    /// <param name="service">Name of the service</param>
    public void LogError(int userId, string message, string callerFilePath, string callerMemberName, int callerLineNumber, string service)
    {
        _logger.Error("User:{UserId} Message:{Message} Service:{Service} Path:{CallerFilePath} Line:{CallerLineNumber} Method:{CallerMemberName}", userId, message,service, callerFilePath, callerLineNumber, callerMemberName);
    }

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="message">The warning message to log.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The member name of the caller.</param>
    public void LogWarning(int userId, string message, string callerFilePath = default, string callerMemberName = default)
    {
        _logger.Warning("User:{UserId} Message:{Message} Path:{CallerFilePath} Method:{CallerMemberName}", userId, message, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs a critical error message.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="message">The critical error message to log.</param>
    /// <param name="callerFilePath">The source file path of the caller.</param>
    /// <param name="callerMemberName">The member name of the caller.</param>
    /// <param name="callerLineNumber">The line number in the source file where the error occurred.</param>
    public void LogCritical(int userId, string message, string callerFilePath, string callerMemberName, int callerLineNumber)
    {
        _logger.Fatal("User:{UserId} Message:{Message} Path:{CallerFilePath} Line:{CallerLineNumber} Method:{CallerMemberName}", userId, message, callerFilePath, callerLineNumber, callerMemberName);
    }
}