using MagicT.Server.ZoneTree.Base;
using MagicT.Server.ZoneTree.Models;

namespace MagicT.Server.ZoneTree.Zones;

public class PermissionsZoneDb : ZoneTreeIntDatabase<List<string>>
{
    public PermissionsZoneDb(string path) : base(path)
    {

    }
}