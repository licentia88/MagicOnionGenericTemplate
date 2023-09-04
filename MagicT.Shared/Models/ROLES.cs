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
    
    // [ForeignKey(nameof(Models.ROLES_D.RD_M_REFNO))]
    // public ICollection<ROLES_D> ROLES_D { get; set; } = new HashSet<ROLES_D>();
}