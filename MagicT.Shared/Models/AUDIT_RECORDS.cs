using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Index( nameof(AR_TABLE_NAME))]
[GenerateDataReaderMapper]
[Table(nameof(AUDIT_RECORDS))]
public partial class AUDIT_RECORDS:AUDIT_BASE
{
    public string AR_TABLE_NAME { get; set; }
 
    [ForeignKey(nameof(Models.AUDIT_RECORDS_D.ARD_M_REFNO))]
    public ICollection<AUDIT_RECORDS_D> AUDIT_RECORDS_D { get; set; } = new HashSet<AUDIT_RECORDS_D>();
}
