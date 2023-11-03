using Coravel.Invocable;
using MagicT.Server.Database;
using MagicT.Server.Enums;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MagicT.Server.Invocables;

public class AuditsInvocable<TContext> : IInvocable, IInvocableWithPayload<AuditTracker> where TContext:DbContext
{
    public TContext Context { get; set; }

    public AuditTracker Payload { get; set; }

    public AuditsInvocable(TContext context)
    {
        Context = context;
    }
    public Task Invoke()
    {
        //Do something
        return Task.CompletedTask;
    }

    private async void OnBeforeSaveChanges()
    {
        var auditEntries = new List<AUDITS>();

        var nowDate = DateTime.Now;

        foreach (var entry in Payload.changeTracker.Entries())
        {
            if (entry.Entity is AUDITS || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            string tableName = entry.Entity.GetType().Name;

            var dbValues = await entry.GetDatabaseValuesAsync();

            foreach (PropertyEntry property in entry.Properties)
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) continue;

                ProcessAudits(property, dbValues, entry.State, tableName, 1, nowDate);

            }
        }

    }


    private void ProcessAudits(PropertyEntry property, PropertyValues propertyValues, EntityState entryState, string tableName,
                            int userId, DateTime nowDate)
    {
        string propertyName = property.Metadata.Name;

        bool isPrimaryKey = property.Metadata.IsPrimaryKey();

        if (entryState == EntityState.Modified && isPrimaryKey) return;
        if (entryState == EntityState.Modified && propertyValues[propertyName].Equals(property.CurrentValue)) return;

        var newAudit = new AUDITS
        {
            AD_TABLE_NAME = tableName,
            AD_IS_PRIMARYKEY = isPrimaryKey,
            AD_DATE = nowDate,
            AD_CURRENT_USER = userId,
            AD_PROPERTY_NAME = propertyName
        };

        if (entryState == EntityState.Added)
        {
            newAudit.AD_TYPE = (int)AuditType.Create;
            newAudit.AD_OLD_VALUE = string.Empty;
            newAudit.AD_NEW_VALUE = property.CurrentValue.ToString();
        }

        if (entryState == EntityState.Modified)
        {
            newAudit.AD_TYPE = (int)AuditType.Update;
            newAudit.AD_OLD_VALUE = propertyValues[propertyName].ToString();
            newAudit.AD_NEW_VALUE = property.CurrentValue.ToString();
        }

        if (entryState == EntityState.Deleted)
        {
            newAudit.AD_TYPE = (int)AuditType.Delete;
            newAudit.AD_OLD_VALUE = property.OriginalValue.ToString();
        }
    }


}

