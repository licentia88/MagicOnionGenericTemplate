using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models.Base;

[MemoryPackable]
[MemoryPackUnion(1, typeof(ROLES))]
[MemoryPackUnion(2, typeof(PERMISSIONS))]
public partial interface IAUTHORIZATIONS_BASE
{
    
}

[Equatable]
[MemoryPackable]
[Table(nameof(AUTHORIZATIONS_BASE))]
public  partial class AUTHORIZATIONS_BASE : IAUTHORIZATIONS_BASE
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AB_ROWID { get; set; }

    public string AB_NAME { get; set; }

    public string AB_AUTH_TYPE { get; set; }
}
