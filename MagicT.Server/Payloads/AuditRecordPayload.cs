using MagicT.Server.Enums;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MagicT.Server.Payloads;

public  class AuditRecordPayload
{
    private string _tableName;

    private DateTime _auditDateTime;

    private EntityEntry _entry;

    private PropertyValues _databaseValues;

    private int _userId;

    private EntityState _entityState;

    private string _service;

    private string _method;

    private string _endpoint;

    public List<AUDIT_RECORDS> AUDIT_RECORDS;

    //public AUDITS_M AUDITS_M { get; set; }

    private Guid _bathcId = Guid.NewGuid();

    public AuditRecordPayload(EntityEntry Entry, int UserId, string service, string method, string endpoint )
    {
        _entry = Entry;

        _service = service;

        _method = method;

        _endpoint = endpoint;

        _userId = UserId;

        _auditDateTime = DateTime.Now;

        _databaseValues = Entry.GetDatabaseValues();

        _tableName = Entry.Entity.GetType().Name;

        _entityState = Entry.State; 

        AUDIT_RECORDS = new();

        //AUDITS_M = new AUDITS_M { AM_DATE = DateTime.Now, AM_USER_ID = UserId, AM_SERVICE = method, AM_SERVICE_ENDPOINT = endpoint };
    }
 
    public void CreateAuditRecord()
    {
        foreach (PropertyEntry property in _entry.Properties)
        {
              ProcessAudits(property);
        }
    }

    private void ProcessAudits(PropertyEntry property)
    {
        string propertyName = property.Metadata.Name;

        bool isPrimaryKey = property.Metadata.IsPrimaryKey();

        if ((_entityState == EntityState.Modified && isPrimaryKey) ||
            (_entityState == EntityState.Modified && _databaseValues[propertyName]!.Equals(property.CurrentValue)))
            return;

        var newAudit = new AUDIT_RECORDS
        {
            AR_TABLE_NAME = _tableName,
            AR_IS_PRIMARYKEY = isPrimaryKey,
            AB_DATE = _auditDateTime,
            AB_USER_ID = _userId,
            AR_PROPERTY_NAME = propertyName,
            AB_SERVICE = _service,
            AB_METHOD = _method,
            AB_END_POINT = _endpoint,
            AR_BATCH_ID = _bathcId
        };

        switch (_entityState)
        {
            case EntityState.Added:
                newAudit.AB_TYPE = (int)AuditType.Create;
                newAudit.AR_OLD_VALUE = string.Empty;
                newAudit.AR_NEW_VALUE = property.CurrentValue?.ToString();
                break;
            case EntityState.Modified:
                newAudit.AB_TYPE = (int)AuditType.Update;
                newAudit.AR_OLD_VALUE = _databaseValues[propertyName]?.ToString();
                if (property.CurrentValue != null) newAudit.AR_NEW_VALUE = property.CurrentValue.ToString();
                break;
            case EntityState.Deleted:
                newAudit.AB_TYPE = (int)AuditType.Delete;
                if (property.OriginalValue != null) newAudit.AR_OLD_VALUE = property.OriginalValue.ToString();
                break;
        }

        //AUDITS_M.AUDIT_BASE.Add(newAudit);
        AUDIT_RECORDS.Add(newAudit);
    }

}
