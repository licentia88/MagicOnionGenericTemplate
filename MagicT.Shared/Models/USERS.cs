using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;

namespace MagicT.Shared.Models
{
    [Equatable]
    [MemoryPackable]
    [Table(nameof(USERS))]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class USERS : USERS_BASE
    {
        public USERS()
        {
            UB_TYPE = nameof(USERS);
        }
    }
}