using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
public partial class USER_ROLES
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UR_ROWID { get; set; }

    public int UR_USER_REFNO { get; set; }

    public int UR_ROLE_REFNO { get; set; }

    [ForeignKey(nameof(UR_ROLE_REFNO))]
    public AUTHORIZATIONS_BASE AUTHORIZATIONS_BASE { get; set; }
}