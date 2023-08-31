using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;
 
namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Table(nameof(ROLES_M))]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class ROLES_M:AUTHORIZATIONS_BASE
{
    public ROLES_M() => AB_AUTH_TYPE = nameof(ROLES_M);
    
    [ForeignKey(nameof(Models.ROLES_D.RD_M_REFNO))]
    public ICollection<ROLES_D> ROLES_D { get; set; } = new HashSet<ROLES_D>();
}