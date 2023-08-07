using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models.Base
{
    [Equatable]
    [MemoryPackable]
    [MemoryPackUnion(1, typeof(USERS))]
    [Table(nameof(USERS_BASE))]
    // ReSharper disable once PartialTypeWithSinglePart
    public abstract partial class USERS_BASE
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UB_ROWID { get; set; }

        public int UB_NAME { get; set; }

        public string UB_TYPE { get; set; } = nameof(USERS_BASE);

        public string UB_SURNAME { get; set; }

        public bool UB_IS_ACTIVE { get; set; } = true;

        [MemoryPackIgnore]
        public string UB_PASSWORD { get; set; }

    }
}