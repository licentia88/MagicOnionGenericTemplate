using Coravel.Queuing.Interfaces;
using Grpc.Core;
using MagicOnion;
using MagicT.Server.Enums;
using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MagicT.Server.Database;

public class MagicTContextAudit : MagicTContext
{
    public int UserId;

    public IQueue Queue { get; set; }

    public MagicTContextAudit(DbContextOptions<MagicTContext> options,IQueue queue) : base(options)
    {
        Queue = queue;
    }

    public DbSet<AUDITS> AUDITS { get; set; }



    private async void OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        
        var auditEntries = new List<AUDITS>();

        var nowDate = DateTime.Now;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AUDITS || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;
            
            string tableName = entry.Entity.GetType().Name;

            var dbValues = await entry.GetDatabaseValuesAsync();

            foreach (PropertyEntry property in entry.Properties)
            {
                if(entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) continue;
                
                ProcessAudits(property, dbValues, entry.State, tableName, UserId, nowDate);
 
            }
        }
        
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        if (UserId is 0)
            throw new ReturnStatusException(StatusCode.NotFound, "User not found");

       

        return base.SaveChangesAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        if (UserId is 0)
            throw new ReturnStatusException(StatusCode.NotFound, "User not found");

        
        var res= await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        OnBeforeSaveChanges();

        return res;
    }

    public override int SaveChanges()
    {
        if (UserId is 0)
            throw new ReturnStatusException(StatusCode.NotFound, "User not found");

        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        if (UserId is 0)
            throw new ReturnStatusException(StatusCode.NotFound, "User not found");

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }


    private void ProcessAudits(PropertyEntry property, PropertyValues propertyValues, EntityState entryState, string tableName,
                               int userId, DateTime nowDate)
    {
        string propertyName = property.Metadata.Name;

        bool isPrimaryKey = property.Metadata.IsPrimaryKey();

        if (entryState == EntityState.Modified  && isPrimaryKey) return;
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
            newAudit.AD_TYPE = (int) AuditType.Create;
            newAudit.AD_OLD_VALUE = string.Empty;
            newAudit.AD_NEW_VALUE = property.CurrentValue.ToString();
        }

        if (entryState == EntityState.Modified)
        {
            newAudit.AD_TYPE = (int) AuditType.Update;
            newAudit.AD_OLD_VALUE = propertyValues[propertyName].ToString();
            newAudit.AD_NEW_VALUE = property.CurrentValue.ToString();
        }
        
        if (entryState == EntityState.Deleted)
        {
            newAudit.AD_TYPE = (int) AuditType.Delete;
            newAudit.AD_OLD_VALUE = property.OriginalValue.ToString();
        }
    }

   
 
    // protected async Task AddAudits()
    // {
    //     ChangeTracker.DetectChanges();
    //
    //
    //     var entriesAdded = ChangedEntryDetails?.Where(x => x.State == EntityState.Added).ToList();
    //     
    //     var entriesModified =ChangedEntryDetails?.Where(x => x.State == EntityState.Modified).ToList();
    //
    //     foreach (var entry in entriesAdded)
    //     {
    //         await base.SaveChangesAsync();
    //         await ProcessCreates(entry);
    //     }
    //
    //     
    //     foreach (var entry in entriesModified) await ProcessUpdates(entry);
    //
    //     await base.SaveChangesAsync();
    // }
    //
    // private IEnumerable<EntityEntry> ChangedEntries =>
    //     ChangeTracker.Entries()
    //     .Where((EntityEntry entry)
    //             => entry.State != EntityState.Detached &&
    //                entry.State != EntityState.Unchanged).ToList();
    //

    // private IEnumerable<(EntityState State, EntityEntry EntityEntry, object Entity, object EntityEntryClone, object DbValues)> ChangedEntryDetails =>
    //     ChangedEntries
    //         .Select(x =>(x.State, x, x.Entity, x.CurrentValues.ToObject(), x.GetDatabaseValues()?.ToObject()?.Clone() ))
    // .ToList();


    // private List<string> GetEntityProperties(object entity) => entity.GetType().GetProperties()
    //               .Where(property => property.PropertyType is {Namespace: "System", IsSerializable: true} && property.CustomAttributes.All(x => x.AttributeType != typeof(NotMappedAttribute)))
    //               .Select(property => property.Name)
    //               .ToList();

    // private async Task ProcessCreates((EntityState State, EntityEntry EntityEntry, object Entity, object EntityEntryClone, object DbValues) entryDetails)
    // {
    //     var tableName = entryDetails.Entity.GetType().Name;
    //
    //     (string key, object value) primaryData = GetPrimaryKey(entryDetails.Entity);
    //     
    //     DateTime nowDate = DateTime.Now;
    //
    //     List<AUDITS> auditDetails = GetEntityProperties(entryDetails.Entity).Select(x =>
    //         new AUDITS
    //         {
    //             
    //             AD_TABLE_NAME = tableName,
    //             AD_DATE = nowDate,
    //             AD_CURRENT_USER = 0,
    //             AD_IS_PRIMARYKEY =  primaryData.key,
    //             AD_PROPERTY_NAME =x,
    //             AD_OLD_VALUE = string.Empty,
    //             AD_NEW_VALUE = entryDetails.Entity.GetPropertyValue(x).ToString(),
    //             AD_TYPE = "C"
    //         }).ToList();
    //
    //      await AUDITS.AddRangeAsync(auditDetails);
    // }
    //
    // private async Task ProcessUpdates((EntityState State, EntityEntry EntityEntry, object Entity, object EntityEntryClone, object DbValues) entryDetails)
    // {
    //     var tableName = entryDetails.Entity.GetType().Name;
    //
    //     (string key, object value) primaryData = GetPrimaryKey(entryDetails.Entity);
    //
    //
    //     DateTime nowDate = DateTime.Now;
    //
    //     List<AUDITS> auditDetails = GetEntityProperties(entryDetails.Entity).Where(x =>
    //     {
    //         var dbValue = entryDetails.DbValues.GetPropertyValue(x);
    //         var newValue = entryDetails.EntityEntryClone.GetPropertyValue(x);
    //         return dbValue is not null && newValue is not null && !dbValue.Equals(newValue);
    //     }).Select(propertyName=> 
    //         new AUDIT_D
    //         {
    //
    //             AD_M_REFNO = existingLogTable.AM_ROWID,
    //             AD_PARENT_NAME = tableName,
    //             AD_DATE = nowDate,
    //             AD_CURRENT_USER = 0,
    //             AD_PRIMARY_KEY = primaryData.key,
    //             AD_PROPERTY_NAME = propertyName,
    //             AD_OLD_VALUE = entryDetails.DbValues.GetPropertyValue(propertyName).ToString(),
    //             AD_NEW_VALUE = entryDetails.EntityEntryClone.GetPropertyValue(propertyName).ToString(),
    //             AD_TYPE = "U"
    //         }).ToList();
    //     
    //     await AUDIT_D.AddRangeAsync(auditDetails);
    //     // foreach (var newHistD in from propertyName in
    //     //              GetEntityProperties(entryDetails.Entity) 
    //     //          let dbValue = entryDetails.DbValues.GetPropertyValue(propertyName) 
    //     //          let newValue = entryDetails.EntityEntryClone.GetPropertyValue(propertyName)
    //     //          where dbValue is not null && newValue is not null && !dbValue.Equals(newValue) select new AUDIT_D
    //     //          {
    //     //
    //     //              AD_M_REFNO = existingLogTable.AM_ROWID,
    //     //              AD_PARENT_NAME = tableName,
    //     //              AD_DATE = nowDate,
    //     //              AD_CURRENT_USER = 0,
    //     //              AD_PRIMARY_KEY = primaryData.key,
    //     //              AD_PROPERTY_NAME = propertyName,
    //     //              AD_OLD_VALUE = dbValue.ToString(),
    //     //              AD_NEW_VALUE = newValue.ToString(),
    //     //              AD_TYPE = "U"
    //     //          })
    //     // {
    //     // }
    //    
    //
    //    
    //     
    //     
    // }
    //
    // private (string key, object value) GetPrimaryKey(object entity)
    // {
    //     var entityType = entity.GetType();
    //     
    //     var entityProperties = entityType.GetProperties();
    //     
    //     var key =  entityProperties
    //         .FirstOrDefault(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(KeyAttribute)))
    //         ?.Name;
    //
    //     if (string.IsNullOrEmpty(key))
    //         throw new ReturnStatusException(StatusCode.Cancelled, "PrimaryKey not found");
    //     
    //     var value = entityProperties.FirstOrDefault(x => x.Name == key)?.GetValue(entity, null);
    //
    //     return (key, value);
    // }
      
}

 