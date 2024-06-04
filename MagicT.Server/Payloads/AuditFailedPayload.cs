using MagicT.Server.Enums;
using MagicT.Shared.Models;

namespace MagicT.Server.Payloads;

public class AuditFailedPayload
{
    public AUDIT_FAILED AuditQuery { get; set; }
 
    public AuditFailedPayload(int userId, string error, string service,  string method, string endPoint, string parameters)
    {
        AuditQuery = new()
        {
            AB_USER_ID = userId,
            AB_METHOD = method,
            AB_SERVICE = service,
            AB_DATE = DateTime.Now,
            AB_TYPE = (int)AuditType.Error,
            AB_END_POINT = endPoint,
            AF_PARAMETERS = parameters,
            AF_ERROR = error,
        };
    }
}
