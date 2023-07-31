using MudBlazor;

namespace MagicT.Web.Models;

public class NotificationVM
{

    public string Message { get; set; }

    public Severity Severity { get; set; }

    public NotificationVM(string message, Severity severity)
    {
        Message = message;
        Severity = severity;
    }

}

