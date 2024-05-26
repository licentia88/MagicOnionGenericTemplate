using MagicOnion;
using Serilog;

namespace MagicT.Shared.Managers;

public class LogManager<TService> where TService:IService<TService>  
{
    public ILogger Logger;

    public LogManager()
    {
        Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console()
           .WriteTo.Map("ServiceName", null, (ServiceName, writer) =>
           {
               var filename = ServiceName ?? "Default";
               writer.File($".Logs/{filename}.Log");
           }).CreateLogger().ForContext("ServiceName",typeof(TService).Name);
          
       
    }

    public void LogMessage(int UserId, string Message,  string CallerFilePath = default, string CallerMemberName = default)
    {
        Task.Run(() =>
        {
            Logger.Information($"User:{UserId}\n" +
                               $"Message:{Message} \n" +
                               $"Path:{CallerFilePath}\n" +
                               $"Method:{CallerMemberName}");
        });
    }

    public void LogError(int UserId, string Message, string CallerFilePath, string CallerMemberName, int CallerLineNumber)
    {
        Task.Run(() =>
        {
            Logger.Information($"User:{UserId}\n" +
                               $"Message:{Message} \n" +
                               $"Path:{CallerFilePath}\n" +
                               $"Line:{CallerLineNumber}" +
                               $"Method:{CallerMemberName}");

        });
    }
}

