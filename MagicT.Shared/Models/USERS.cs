using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicT.Shared.Models;

[Equatable()]
[GenerateDataReaderMapper]
[MemoryPackable]
// ReSharper disable once PartialTypeWithSinglePart
public  partial class USERS : IValidatableObject
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int U_ROWID { get; set; }

    public string U_FULLNAME { get; set; }
    
    public string U_USERNAME { get; set; }  

    public bool U_IS_ACTIVE { get; set; } = true;

    public string U_PASSWORD { get; set; }
 
    [Required]
    [MaxLength(30)]
    public string U_NAME { get; set; }

    [Required]
    [MaxLength(30)]
    public string U_LASTNAME { get; set; }

    [Required]
    [MaxLength(15)]
    public string U_PHONE_NUMBER { get; set; }

    [Required]
    [MaxLength(30)]
    public string U_EMAIL { get; set; }

    public bool U_IS_ADMIN { get; set; }

    [IgnoreEquality]
    [ForeignKey(nameof(Models.USER_ROLES.UR_USER_REFNO))]
    public ICollection<USER_ROLES> USER_ROLES { get; set; } = new HashSet<USER_ROLES>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
       U_FULLNAME = $"{U_NAME} {U_LASTNAME}";

       return new List<ValidationResult>();
    }
}