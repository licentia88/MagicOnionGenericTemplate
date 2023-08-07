using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;

namespace MagicT.Shared.Models.Base;

    [Equatable]
    [MemoryPackable]
    [MemoryPackUnion(1, typeof(ROLES))]
    [Table(nameof(AUTHORIZATIONS_BASE))]
    public abstract partial class AUTHORIZATIONS_BASE
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AB_ROWID { get; set; }

        public int AB_USER_REFNO { get; set; }
        
        public string AB_AUTH_TYPE { get; set; }
        
        public string AB_DESCRIPTION { get; set; }
    }
