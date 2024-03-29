using MudBlazor;

namespace MagicT.Web.Shared.Models;

// ReSharper disable once InconsistentNaming
public class NotificationVM
{
    public NotificationVM(string message, Severity severity)
    {
        Message = message;
        Severity = severity;
    }

    public string Message { get; }

    public Severity Severity { get; }
}