namespace MagicT.Server.Payloads;


public class AuditQueryPayload 
{
    public global::MagicT.Shared.Models.AUDIT_QUERY AuditQuery { get; set; }

    public AuditQueryPayload(int userId, string service, string method, string endPoint, string parameters)
    {
        AuditQuery = new()
        {
            AB_USER_ID = userId,
            AB_METHOD = method,
            AB_SERVICE = service,
            AB_DATE = global::System.DateTime.Now,
            AB_TYPE = (int)global::MagicT.Server.Enums.AuditType.Query,
            AB_END_POINT = endPoint,
            AQ_PARAMETERS = parameters,
        };
    }

    
}