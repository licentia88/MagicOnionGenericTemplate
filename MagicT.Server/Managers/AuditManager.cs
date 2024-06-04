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
    private IQueue Queue;

    [EnableAutomaticDispose]
    public readonly CancellationTokenManager CancellationTokenManager;

    Func<Guid> TaskQueue;

    public AuditManager(IServiceProvider provider)
    {
        provider = provider.CreateScope().ServiceProvider;
        CancellationTokenManager = provider.GetService<CancellationTokenManager>();
        Queue = provider.GetService<IQueue>();
    }

    public void AuditRecords(ServiceContext serviceContext, IEnumerable<EntityEntry> entityEntries, int Id)
    {
        foreach (EntityEntry entity in entityEntries)
        {
            var payload = new AuditRecordPayload(entity, Id, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method);
            TaskQueue = () => Queue.QueueInvocableWithPayload<AuditRecordsInvocable<MagicTContext>, AuditRecordPayload>(payload);
        }
    }

    public void AuditRecords(ServiceContext serviceContext, IEnumerable<EntityEntry> entityEntries)
    {
        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var Id = token is null ? 0 : token.Id;

        AuditRecords(serviceContext, entityEntries, Id);
    }

   
    public void AuditQueries(ServiceContext serviceContext, int Id, params object[] parameters)
    {
        var loParams = JsonSerializer.Serialize(parameters);
        var payload = new AuditQueryPayload(Id, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method, loParams);
        TaskQueue = ()=> Queue.QueueInvocableWithPayload<AuditQueryInvocable<MagicTContext>, AuditQueryPayload>(payload);
    }

    public void AuditQueries(ServiceContext serviceContext, params object[] parameters)
    {
        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var Id = token is null ? 0 : token.Id;

        AuditQueries(serviceContext, Id, parameters);
    }

    public void AuditQueries(ServiceContext serviceContext, params byte[] parameterBytes)
    {
        var parameters = parameterBytes?.DeserializeFromBytes<KeyValuePair<string, object>[]>();

        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var Id = token is null ? 0 : token.Id;

        AuditQueries(serviceContext, Id, parameters);
    }

    public void AuditFailed(ServiceContext serviceContext, int Id, string error, params object[] parameters)
    {
        var loParams = JsonSerializer.Serialize(parameters);
        var payload = new AuditFailedPayload(Id,error, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method, loParams);
        TaskQueue = () => Queue.QueueInvocableWithPayload<AuditFailedInvocable<MagicTContext>, AuditFailedPayload>(payload);
    }

    public void AuditFailed(ServiceContext serviceContext,string error, params object[] parameters)
    {
        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var Id = token is null ? 0 : token.Id;

        AuditFailed(serviceContext, Id, error, parameters);
    }

    public void AuditFailed(ServiceContext serviceContext, string error, params byte[] parameterBytes)
    {
        var parameters = parameterBytes?.DeserializeFromBytes<KeyValuePair<string, object>[]>();

        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));

        var Id = token is null ? 0 : token.Id;

        AuditFailed(serviceContext, Id,error, parameters);
    }

    public void SaveChanges()
    {
        TaskQueue?.Invoke();
    }

    

    //public void Dispose()
    //{
    //    Queue = null;
    //    TaskQueue = null;
    //    //CancellationTokenManager = null;
    //}


}
