using ImmutableObjectGraph.Generation;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MagicT.Server.Database;

[GenerateImmutable]
public partial class AuditTracker
{
    public ChangeTracker changeTracker { get; private set; }

    public AuditTracker(ChangeTracker tracker)
    {
        changeTracker = tracker;
    }
}
