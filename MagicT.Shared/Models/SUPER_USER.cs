using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Table(nameof(SUPER_USER))]
public partial class SUPER_USER: USERS
{
    public SUPER_USER() => UB_TYPE = nameof(SUPER_USER);

}
