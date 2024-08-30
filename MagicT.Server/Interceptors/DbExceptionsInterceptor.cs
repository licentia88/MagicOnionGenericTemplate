namespace MagicT.Server.Interceptors;


[global::RegisterSingleton]
internal sealed class DbExceptionsInterceptor : global::Microsoft.EntityFrameworkCore.Diagnostics.SaveChangesInterceptor
{
    public override global::System.Threading.Tasks.Task SaveChangesFailedAsync(global::Microsoft.EntityFrameworkCore.Diagnostics.DbContextErrorEventData eventData, global::System.Threading.CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null) global::MagicT.Server.Interceptors.DbExceptionsInterceptor.ProcessExceptions(eventData.Context);
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    public override void SaveChangesFailed(global::Microsoft.EntityFrameworkCore.Diagnostics.DbContextErrorEventData eventData)
    {
        if (eventData.Context is not null) global::MagicT.Server.Interceptors.DbExceptionsInterceptor.ProcessExceptions(eventData.Context);
        base.SaveChangesFailed(eventData);
    }

    private static void ProcessExceptions(global::Microsoft.EntityFrameworkCore.DbContext context)
    {
     
    }
}

