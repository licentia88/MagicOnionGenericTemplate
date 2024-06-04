using MagicT.Server.Enums;
using MagicT.Shared.Models;

namespace MagicT.Server.Payloads;


public class AuditQueryPayload 
{
    public AUDIT_QUERY AuditQuery { get; set; }

    public AuditQueryPayload(int userId, string service, string method, string endPoint, string parameters)
    {
        AuditQuery = new()
        {
            AB_USER_ID = userId,
            AB_METHOD = method,
            AB_SERVICE = service,
            AB_DATE = DateTime.Now,
            AB_TYPE = (int)AuditType.Query,
            AB_END_POINT = endPoint,
            AQ_PARAMETERS = parameters,
        };
    }

    
}