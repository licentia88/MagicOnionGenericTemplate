using MagicT.Server.Enums;
using MagicT.Shared.Models;

namespace MagicT.Server.Database;

public class AuditFailedPayload
{
    public AUDIT_FAILED AuditQuery { get; set; }


    public AuditFailedPayload(int userId, string service, string method, string endPoint, string parameters)
    {

        AuditQuery = new()
        {
            AB_USER_ID = userId,
            AB_METHOD = method,
            AB_SERVICE = service,
            AB_DATE = DateTime.Now,
            AB_TYPE = (int)AuditType.Error,
            AB_END_POINT = endPoint,
            AF_PARAMETERS = parameters
        };
    }

    //public AuditFailedPayload(string service, string error, string endPoint, string parameters) : this(0, service, error, endPoint, parameters)
    //{

    //}



}
