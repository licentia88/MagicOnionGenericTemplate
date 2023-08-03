using MudBlazor;

namespace MagicT.Web.Models;

// ReSharper disable once InconsistentNaming
public class NotificationVM
{
    public string Message { get; }

    public Severity Severity { get; }

    public NotificationVM(string message, Severity severity)
    {
        Message = message;
        Severity = severity;
    }

}

