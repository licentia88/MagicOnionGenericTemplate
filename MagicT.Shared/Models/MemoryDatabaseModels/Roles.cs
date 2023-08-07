using MasterMemory;
using MessagePack;

namespace MagicT.Shared.Models.MemoryDatabaseModels;

[MemoryTable(nameof(Roles)), MessagePackObject(true)]
public class Roles : Authorizations
{
    [PrimaryKey]
    public override int Id { get; set; }

    [SecondaryKey(0, 1)]
    public override int UserRefNo { get; set; }

    [SecondaryKey(1, 1)]
    public override string AuthType { get; set; }

    // You can add additional properties specific to Permissions here
}

