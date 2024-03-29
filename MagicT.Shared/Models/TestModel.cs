﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MemoryPack;
 

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class TestModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Description { get; set; }

    public string DescriptionDetails { get; set; }

    public string CheckData { get; set; }
  
}
