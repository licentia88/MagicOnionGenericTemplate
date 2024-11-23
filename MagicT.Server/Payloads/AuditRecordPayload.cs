using Benutomo;
using MagicT.Server.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ModelExtensions = MagicT.Shared.Extensions.ModelExtensions;

namespace MagicT.Server.Payloads;

/// <summary>
/// Represents the payload for an audit record operation.
/// </summary>
[AutomaticDisposeImpl]
public partial class AuditRecordPayload:IDisposable
{
    private readonly string _tableName;
    private readonly DateTime _auditDateTime;
    private readonly EntityEntry _entry;
    private readonly PropertyValues _databaseValues;
    private readonly int _userId;
    private readonly EntityState _entityState;
    private readonly string _service;
    private readonly string _method;
    private readonly string _endpoint;

    /// <summary>
    /// Gets the audit records details.
    /// </summary>
    [EnableAutomaticDispose]
    public readonly AUDIT_RECORDS AuditRecords;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuditRecordPayload"/> class.
    /// </summary>
    /// <param name="entry">The entity entry being audited.</param>
    /// <param name="userId">The user ID associated with the audit.</param>
    /// <param name="service">The service name where the audit is performed.</param>
    /// <param name="method">The method name where the audit is performed.</param>
    /// <param name="endpoint">The endpoint associated with the audit.</param>
    public AuditRecordPayload(EntityEntry entry, int userId, string service, string method, string endpoint)
    {
        _entry = entry;
        _service = service;
        _method = method;
        _endpoint = endpoint;
        _userId = userId;
        _auditDateTime = DateTime.Now;
        _databaseValues = entry.GetDatabaseValues();
        _tableName = entry.Entity.GetType().Name;
        _entityState = entry.State;
        AuditRecords = new AUDIT_RECORDS();
    }

    ~AuditRecordPayload()
    {
        Dispose(false);
    }
    /// <summary>
    /// Creates the audit record.
    /// </summary>
    public void CreateAuditRecord()
    {
        foreach (PropertyEntry property in _entry.Properties)
        {
            ProcessAudits(property);
        }
    }

    /// <summary>
    /// Processes the audits for a given property.
    /// </summary>
    /// <param name="property">The property entry being audited.</param>
    private void ProcessAudits(PropertyEntry property)
    {
        string propertyName = property.Metadata.Name;
        bool isPrimaryKey = property.Metadata.IsPrimaryKey();

        if (_entityState == EntityState.Modified && ShouldSkipProperty(property, propertyName, isPrimaryKey))
        {
            return;
        }

        SetAuditRecordDetails();
        AddAuditRecordDetail(property, propertyName, isPrimaryKey);
    }

    /// <summary>
    /// Determines if the property should be skipped during audit processing.
    /// </summary>
    /// <param name="property">The property entry being audited.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="isPrimaryKey">Indicates if the property is a primary key.</param>
    /// <returns>True if the property should be skipped; otherwise, false.</returns>
    private bool ShouldSkipProperty(PropertyEntry property, string propertyName, bool isPrimaryKey)
    {
        if (isPrimaryKey) return true;

        var dbValue = _databaseValues[propertyName];
        var currentValue = property.CurrentValue;

        return (dbValue == null && currentValue == null) || (dbValue != null && dbValue.Equals(currentValue));
    }

    /// <summary>
    /// Sets the details for the audit record.
    /// </summary>
    private void SetAuditRecordDetails()
    {
        AuditRecords.AR_TABLE_NAME = _tableName;
        AuditRecords.AB_DATE = _auditDateTime;
        AuditRecords.AB_USER_ID = _userId;
        AuditRecords.AB_SERVICE = _service;
        AuditRecords.AB_METHOD = _method;
        AuditRecords.AB_END_POINT = _endpoint;

        var primaryKey = ModelExtensions.GetPrimaryKey(_entry.Entity.GetType());
        AuditRecords.AR_PK_VALUE = _entry.Entity.GetPropertyValue(primaryKey).ToString();
    }

    /// <summary>
    /// Adds the audit record detail for a given property.
    /// </summary>
    /// <param name="property">The property entry being audited.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="isPrimaryKey">Indicates if the property is a primary key.</param>
    private void AddAuditRecordDetail(PropertyEntry property, string propertyName, bool isPrimaryKey)
    {
        var newAudit = new AUDIT_RECORDS_D
        {
            ARD_IS_PRIMARYKEY = isPrimaryKey,
            ARD_PROPERTY_NAME = propertyName,
        };

        switch (_entityState)
        {
            case EntityState.Added:
                AuditRecords.AB_TYPE = (int)AuditType.Create;
                newAudit.ARD_OLD_VALUE = string.Empty;
                newAudit.ARD_NEW_VALUE = property.CurrentValue?.ToString();
                break;
            case EntityState.Modified:
                AuditRecords.AB_TYPE = (int)AuditType.Update;
                newAudit.ARD_OLD_VALUE = _databaseValues[propertyName]?.ToString();
                newAudit.ARD_NEW_VALUE = property.CurrentValue?.ToString();
                break;
            case EntityState.Deleted:
                AuditRecords.AB_TYPE = (int)AuditType.Delete;
                newAudit.ARD_OLD_VALUE = property.OriginalValue?.ToString();
                break;
        }

        AuditRecords.AUDIT_RECORDS_D.Add(newAudit);
    }
}