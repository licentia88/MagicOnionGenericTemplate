using System.Text.Json;
using Benutomo;
using Coravel.Queuing.Interfaces;
using MagicOnion.Server;
using MagicT.Server.Extensions;
using MagicT.Server.Invocables;
using MagicT.Server.Jwt;
using MagicT.Server.Payloads;
using MagicT.Shared.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MagicT.Server.Managers;

/// <summary>
/// Manages audit operations including records, queries, and failed audits.
/// </summary>
[AutomaticDisposeImpl]
public partial class AuditManager : IDisposable, IAsyncDisposable
{
    private readonly IQueue _queue;

    [EnableAutomaticDispose]
    private readonly CancellationTokenManager _cancellationTokenManager;

    private Func<Guid> _taskQueue;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditManager"/> class.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    public AuditManager(IServiceProvider provider)
    {
        provider = provider.CreateScope().ServiceProvider;
        _cancellationTokenManager = provider.GetService<CancellationTokenManager>();
        _queue = provider.GetService<IQueue>();
    }

    /// <summary>
    /// Retrieves the user ID from the service context.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <returns>The user ID.</returns>
    private int GetUserId(ServiceContext serviceContext)
    {
        var token = serviceContext.GetItemAs<MagicTToken>(nameof(MagicTToken));
        return token?.Id ?? 0;
    }

    /// <summary>
    /// Serializes the parameters to a JSON string.
    /// </summary>
    /// <param name="parameters">The parameters to serialize.</param>
    /// <returns>The serialized parameters as a JSON string.</returns>
    private string SerializeParameters(object[] parameters)
    {
        return JsonSerializer.Serialize(parameters);
    }

    /// <summary>
    /// Audits the records by creating audit record payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="entityEntries">The entity entries to audit.</param>
    /// <param name="id">The user ID.</param>
    public void AuditRecords(ServiceContext serviceContext, IEnumerable<EntityEntry> entityEntries, int id)
    {
        foreach (EntityEntry entity in entityEntries)
        {
            var payload = new AuditRecordPayload(entity, id, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method);
            _taskQueue = () => _queue.QueueInvocableWithPayload<AuditRecordsInvocable<MagicTContext>, AuditRecordPayload>(payload);
        }
    }

    /// <summary>
    /// Audits the records by creating audit record payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="entityEntries">The entity entries to audit.</param>
    public void AuditRecords(ServiceContext serviceContext, IEnumerable<EntityEntry> entityEntries)
    {
        AuditRecords(serviceContext, entityEntries, GetUserId(serviceContext));
    }

    /// <summary>
    /// Audits the queries by creating audit query payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="id">The user ID.</param>
    /// <param name="parameters">The parameters to audit.</param>
    public void AuditQueries(ServiceContext serviceContext, int id, params object[] parameters)
    {
        var loParams = SerializeParameters(parameters);
        var payload = new AuditQueryPayload(id, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method, loParams);
        _taskQueue = () => _queue.QueueInvocableWithPayload<AuditQueryInvocable<MagicTContext>, AuditQueryPayload>(payload);
    }

    /// <summary>
    /// Audits the queries by creating audit query payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="parameters">The parameters to audit.</param>
    public void AuditQueries(ServiceContext serviceContext, params object[] parameters)
    {
        AuditQueries(serviceContext, GetUserId(serviceContext), parameters);
    }

    /// <summary>
    /// Audits the queries by creating audit query payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="parameterBytes">The parameters to audit as a byte array.</param>
    public void AuditQueries(ServiceContext serviceContext, params byte[] parameterBytes)
    {
        var parameters = parameterBytes?.DeserializeFromBytes<KeyValuePair<string, object>[]>();
        AuditQueries(serviceContext, GetUserId(serviceContext), parameters);
    }

    /// <summary>
    /// Audits the failed operations by creating audit failed payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="id">The user ID.</param>
    /// <param name="error">The error message.</param>
    /// <param name="parameters">The parameters to audit.</param>
    public void AuditFailed(ServiceContext serviceContext, int id, string error, params object[] parameters)
    {
        var loParams = SerializeParameters(parameters);
        var payload = new AuditFailedPayload(id, error, serviceContext.ServiceType.Name, serviceContext.MethodInfo.Name, serviceContext.CallContext.Method, loParams);
        _taskQueue = () => _queue.QueueInvocableWithPayload<AuditFailedInvocable<MagicTContext>, AuditFailedPayload>(payload);
    }

    /// <summary>
    /// Audits the failed operations by creating audit failed payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="error">The error message.</param>
    /// <param name="parameters">The parameters to audit.</param>
    public void AuditFailed(ServiceContext serviceContext, string error, params object[] parameters)
    {
        AuditFailed(serviceContext, GetUserId(serviceContext), error, parameters);
    }

    /// <summary>
    /// Audits the failed operations by creating audit failed payloads and queuing them.
    /// </summary>
    /// <param name="serviceContext">The service context.</param>
    /// <param name="error">The error message.</param>
    /// <param name="parameterBytes">The parameters to audit as a byte array.</param>
    public void AuditFailed(ServiceContext serviceContext, string error, params byte[] parameterBytes)
    {
        var parameters = parameterBytes?.DeserializeFromBytes<KeyValuePair<string, object>[]>();
        AuditFailed(serviceContext, GetUserId(serviceContext), error, parameters);
    }

    /// <summary>
    /// Saves the changes by invoking the queued task.
    /// </summary>
    public void SaveChanges()
    {
        _taskQueue?.Invoke();
    }
}