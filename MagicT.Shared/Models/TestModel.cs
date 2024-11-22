using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Benutomo;


namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[AutomaticDisposeImpl]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class TestModel:IDisposable, IAsyncDisposable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }

    //[MaxLength(2)]
    public string DescriptionDetails { get; set; }

    public string MediaDescription { get; set; }

    public string CheckData { get; set; }

    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public bool IsTrue { get; set; }
    
    
}
