using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Index(nameof(AD_DATE), nameof(AD_CURRENT_USER), nameof(AD_TABLE_NAME), nameof(AD_PROPERTY_NAME), nameof(AD_TYPE))]
public partial class AUDITS
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AD_ROWID { get; set; }
    
    public string AD_TABLE_NAME { get; set; }

    public bool AD_IS_PRIMARYKEY { get; set; }

    public DateTime AD_DATE { get; set; }

    public int AD_CURRENT_USER { get; set; }
    
    public string AD_PROPERTY_NAME { get; set; }

    public string AD_OLD_VALUE { get; set; }

    public string AD_NEW_VALUE { get; set; }

    public int AD_TYPE { get; set; }
}
