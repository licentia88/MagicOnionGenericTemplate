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

    public int UR_AUTH_CODE { get; set; }

    [ForeignKey(nameof(UR_AUTH_CODE))]
    public AUTHORIZATIONS_BASE AUTHORIZATIONS_BASE { get; set; }
}