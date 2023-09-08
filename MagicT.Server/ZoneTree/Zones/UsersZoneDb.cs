using MagicT.Server.ZoneTree.Base;
using MagicT.Server.ZoneTree.Models;

namespace MagicT.Server.ZoneTree.Zones;

public class UsersZoneDb : ZoneTreeIntDatabase<UsersZone>
{
    public UsersZoneDb(string path) : base(path)
    {

    }
}
