using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[MemoryPackUnion(1,typeof(AUDIT_QUERY))]
[MemoryPackUnion(2,typeof(AUDIT_RECORDS))]
[MemoryPackUnion(3, typeof(AUDIT_FAILED))]
[Table(nameof(AUDIT_BASE)),Index(nameof(AB_DATE), nameof(AB_TYPE), nameof(AB_USER_ID), nameof(AB_SERVICE), nameof(AB_METHOD))]
public abstract partial class AUDIT_BASE
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AB_ROWID { get; set; }

    public DateTime AB_DATE { get; set; }

    public int AB_TYPE { get; set; }

    public int AB_USER_ID { get; set; }

    public string AB_SERVICE { get; set; }

    public string AB_METHOD { get; set; }

    public string AB_END_POINT { get; set; }

    
}

 

 
 