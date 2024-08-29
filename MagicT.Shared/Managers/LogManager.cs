using MagicOnion;
using Serilog;

namespace MagicT.Shared.Managers;

public class LogManager<TService> where TService:IService<TService>  
{
    private readonly ILogger _logger;

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

    public void LogMessage(int userId, string message,  string callerFilePath = default, string callerMemberName = default)
    {
        _logger.Information("User:{UserId} Message:{Message} Path:{CallerFilePath} Method:{CallerMemberName}",userId,message,callerFilePath,callerMemberName);

    }

    public void LogError(int userId, string message, string callerFilePath, string callerMemberName, int callerLineNumber)
    {
        _logger.Information("User:{UserId} Message:{Message} Path:{CallerFilePath} Line:{CallerLineNumber}Method:{CallerMemberName}",userId,message,callerFilePath,callerLineNumber,callerMemberName);
    }
}

