using MasterMemory;
using MessagePack;

namespace MagicT.Shared.Models.MemoryDatabaseModels;

[MemoryTable(nameof(Users)), MessagePackObject(true)]
public partial class Users
{
    [PrimaryKey]
    public int UserId { get; set; }
    
    [SecondaryKey(2,2)]
    public string ContactIdentifier { get; set; }

    [SecondaryKey(1,1)]
    public byte[] SharedKey { get; set; }
}
