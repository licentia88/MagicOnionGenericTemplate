using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MagicT.Server.Interceptors;


[RegisterSingleton]
internal sealed class DbExceptionsInterceptor : SaveChangesInterceptor
{
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null) ProcessExceptions(eventData.Context);
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (eventData.Context is not null) ProcessExceptions(eventData.Context);
        base.SaveChangesFailed(eventData);
    }

    private static void ProcessExceptions(DbContext context)
    {
     
    }
}

