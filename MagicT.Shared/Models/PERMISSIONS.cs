using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Table(nameof(PERMISSIONS))]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class PERMISSIONS : AUTHORIZATIONS_BASE ,IValidatableObject
{
    public PERMISSIONS() =>AB_AUTH_TYPE = nameof(PERMISSIONS);

    public string PER_PERMISSION_NAME { get; set; }

    public string PER_ROLE_NAME { get; set; }

    public string PER_IDENTIFIER_NAME { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        PER_IDENTIFIER_NAME = $"{PER_ROLE_NAME}/{PER_PERMISSION_NAME}";
        return new List<ValidationResult>();
    }
}