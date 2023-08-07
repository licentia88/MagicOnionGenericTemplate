using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Table(nameof(ROLES))]
// ReSharper disable once PartialTypeWithSinglePart
public partial class ROLES:AUTHORIZATIONS_BASE
{
    public ROLES() => AB_AUTH_TYPE = nameof(ROLES);

 
}