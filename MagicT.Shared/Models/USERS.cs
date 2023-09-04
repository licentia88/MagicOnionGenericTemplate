using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Generator.Equals;
using MagicT.Shared.Models.Base;
using MemoryPack;

namespace MagicT.Shared.Models;

[Equatable]
[MemoryPackable]
[Table(nameof(USERS))]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class USERS : USERS_BASE ,IValidatableObject
{
    /// <summary>
    ///    Initializes a new instance of the <see cref="USERS" /> class.
    /// </summary>
    public USERS() => UB_TYPE = nameof(USERS);

    [Required]
    [MaxLength(30)]
    public string U_NAME { get; set; }

    [Required]
    [MaxLength(30)]
    public string U_SURNAME { get; set; }

    [Required]
    [MaxLength(15)]
    public string U_PHONE_NUMBER { get; set; }

    [Required]
    [MaxLength(30)]
    public string U_EMAIL { get; set; }

    //[ForeignKey(nameof(Models.USER_ROLES.UR_USER_REFNO))]
    //public ICollection<USER_ROLES> USER_ROLES { get; set; } = new HashSet<USER_ROLES>();


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
       UB_FULLNAME = $"{U_NAME} {U_SURNAME}";

       return new List<ValidationResult>();
    }
}