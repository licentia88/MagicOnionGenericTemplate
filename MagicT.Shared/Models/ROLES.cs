using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;

namespace MagicT.Shared.Models;


[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[Table(nameof(ROLES))]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class ROLES : AUTHORIZATIONS_BASE,IAUTHORIZATIONS_BASE
{
    public ROLES() => AB_AUTH_TYPE = nameof(ROLES);

    [ForeignKey(nameof(Models.PERMISSIONS.PER_ROLE_REFNO))]
    public ICollection<PERMISSIONS> PERMISSIONS { get; set; } = new HashSet<PERMISSIONS>();

}

 