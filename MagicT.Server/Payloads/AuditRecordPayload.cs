using MagicT.Server.Enums;
using MagicT.Shared.Extensions;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static Grpc.Core.Metadata;

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

    public AUDIT_RECORDS AUDIT_RECORDS;

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

        if(_entityState == EntityState.Modified)
        {
            if (isPrimaryKey) return;

            var dbValue = _databaseValues[propertyName];
            var currentValue = property.CurrentValue;

            if (dbValue == null && currentValue == null) return;

            if (dbValue.Equals(currentValue)) return;

        }


        AUDIT_RECORDS.AR_TABLE_NAME = _tableName;
        AUDIT_RECORDS.AB_DATE = _auditDateTime;
        AUDIT_RECORDS.AB_USER_ID = _userId;
        AUDIT_RECORDS.AB_SERVICE = _service;
        AUDIT_RECORDS.AB_METHOD = _method;
        AUDIT_RECORDS.AB_END_POINT = _endpoint;

        var primaryKey = Shared.Extensions.ModelExtensions.GetPrimaryKey(_entry.Entity.GetType());
        AUDIT_RECORDS.AR_PK_VALUE = _entry.Entity.GetPropertyValue(primaryKey).ToString();
          

        var newAudit = new AUDIT_RECORDS_D
        {
            ARD_IS_PRIMARYKEY = isPrimaryKey,
            ARD_PROPERTY_NAME = propertyName,
           
        };

        switch (_entityState)
        {
            case EntityState.Added:
                AUDIT_RECORDS.AB_TYPE = (int)AuditType.Create;
                newAudit.ARD_OLD_VALUE = string.Empty;
                newAudit.ARD_NEW_VALUE = property.CurrentValue?.ToString();
                break;
            case EntityState.Modified:
                AUDIT_RECORDS.AB_TYPE = (int)AuditType.Update;
                newAudit.ARD_OLD_VALUE = _databaseValues[propertyName]?.ToString();
                if (property.CurrentValue != null) newAudit.ARD_NEW_VALUE = property.CurrentValue.ToString();
                break;
            case EntityState.Deleted:
                AUDIT_RECORDS.AB_TYPE = (int)AuditType.Delete;
                if (property.OriginalValue != null) newAudit.ARD_OLD_VALUE = property.OriginalValue.ToString();
                break;
        }

        //AUDITS_M.AUDIT_BASE.Add(newAudit);
        AUDIT_RECORDS.AUDIT_RECORDS_D.Add(newAudit);
    }

}
