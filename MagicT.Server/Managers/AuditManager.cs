using System.Text.Json;
using Benutomo;
using Coravel.Queuing.Interfaces;
using MagicOnion.Server;
using MagicT.Server.Database;
using MagicT.Server.Extensions;
using MagicT.Server.Invocables;
using MagicT.Server.Jwt;
using MagicT.Server.Payloads;
using MagicT.Shared.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MagicT.Server.Managers;

[AutomaticDisposeImpl]
public partial class AuditManager : IDisposable, IAsyncDisposable
{
    private IQueue _queue;

    [EnableAutomaticDispose]
    public readonly CancellationTokenManager CancellationTokenManager;

    Func<Guid> _taskQueue;

    public AuditManager(IServiceProvider provider)
    {
        provider = provider.CreateScope().ServiceProvider;
        CancellationTokenManager = provider.GetService<CancellationTokenManager>();
        _queue = provider.GetService<IQueue>();
    }

    public void AuditRecords(ServiceContext serviceContext, IEnumerable<EntityEntry> entityEntries, int id)
    {
        foreach (EntityEntry entity in entityEntries)
        {
            var payload = new AuditRecordPayload(entity, id, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method);
            _taskQueue = () => _queue.QueueInvocableWithPayload<AuditRecordsInvocable<MagicTContext>, AuditRecordPayload>(payload);
        }
    }

    public void AuditRecords(ServiceContext serviceContext, IEnumerable<EntityEntry> entityEntries)
    {
        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var id = token?.Id ?? 0;

        AuditRecords(serviceContext, entityEntries, id);
    }

   
    public void AuditQueries(ServiceContext serviceContext, int id, params object[] parameters)
    {
        var loParams = JsonSerializer.Serialize(parameters);
        var payload = new AuditQueryPayload(id, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method, loParams);
        _taskQueue = ()=> _queue.QueueInvocableWithPayload<AuditQueryInvocable<MagicTContext>, AuditQueryPayload>(payload);
    }

    public void AuditQueries(ServiceContext serviceContext, params object[] parameters)
    {
        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var id = token?.Id ?? 0;

        AuditQueries(serviceContext, id, parameters);
    }

    public void AuditQueries(ServiceContext serviceContext, params byte[] parameterBytes)
    {
        var parameters = parameterBytes?.DeserializeFromBytes<KeyValuePair<string, object>[]>();

        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var id = token?.Id ?? 0;

        AuditQueries(serviceContext, id, parameters);
    }

    public void AuditFailed(ServiceContext serviceContext, int id, string error, params object[] parameters)
    {
        var loParams = JsonSerializer.Serialize(parameters);
        var payload = new AuditFailedPayload(id,error, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method, loParams);
        _taskQueue = () => _queue.QueueInvocableWithPayload<AuditFailedInvocable<MagicTContext>, AuditFailedPayload>(payload);
    }

    public void AuditFailed(ServiceContext serviceContext,string error, params object[] parameters)
    {
        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var id = token?.Id ?? 0;

        AuditFailed(serviceContext, id, error, parameters);
    }

    public void AuditFailed(ServiceContext serviceContext, string error, params byte[] parameterBytes)
    {
        var parameters = parameterBytes?.DeserializeFromBytes<KeyValuePair<string, object>[]>();

        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var id = token?.Id ?? 0;

        AuditFailed(serviceContext, id,error, parameters);
    }

    public void SaveChanges()
    {
        _taskQueue?.Invoke();
    }

    

    //public void Dispose()
    //{
    //    Queue = null;
    //    TaskQueue = null;
    //    //CancellationTokenManager = null;
    //}


}
