using MagicT.Server.ZoneTree.Base;
using MagicT.Server.ZoneTree.Models;

namespace MagicT.Server.ZoneTree.Zones;


public class UsedTokensZoneDb : ZoneTreeIntDatabase<List<UsedTokensZone>>
{
    public UsedTokensZoneDb(string path) : base(path)
    {

    }


    public virtual void AddOrUpdate(long id, UsedTokensZone model)
    {
        Database.TryAtomicAddOrUpdate(id, new List<UsedTokensZone> { model}, (ref List<UsedTokensZone> value) =>
        {
            value ??= new List<UsedTokensZone>();
            value.Add(model);
            return true;
        });

    }
    
     
}
