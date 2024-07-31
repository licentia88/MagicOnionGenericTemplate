using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class TestModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }

    //[MaxLength(2)]
    public string DescriptionDetails { get; set; }

    public string CheckData { get; set; }
  
}
