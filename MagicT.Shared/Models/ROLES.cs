using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;
 
namespace MagicT.Shared.Models;


[Equatable]
[MemoryPackable]
[Table(nameof(ROLES))]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class ROLES : AUTHORIZATIONS_BASE
{
    public ROLES() => AB_AUTH_TYPE = nameof(ROLES);

    [ForeignKey(nameof(Models.PERMISSIONS.PER_ROLE_REFNO))]
    public ICollection<PERMISSIONS> PERMISSIONS { get; set; } = new HashSet<PERMISSIONS>();

}

//[Equatable]
//[MemoryPackable]
//[Table(nameof(ROLES))]
//public sealed partial class ROLES_M : AUTHORIZATIONS_BASE
//{
//    public ROLES_M() => AB_AUTH_TYPE = nameof(ROLES_M);
//}

//[Equatable]
//[MemoryPackable]
//public sealed  partial class ROLES_D
//{
//    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//    public int RD_ROWID { get; set; }

//    public int RD_M_REFNO { get; set; }

//    public int? RD_PERMISSION_REFNO { get; set; }

//    [ForeignKey(nameof(RD_PERMISSION_REFNO))]
//    public PERMISSIONS PERMISSIONS { get; set; }
//}