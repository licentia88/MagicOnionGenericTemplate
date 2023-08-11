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
   
    public string U_NAME { get; set; }
    
    public string U_SURNAME { get; set; }

    public string U_PHONE_NUMBER { get; set; }
    
    public string U_EMAIL { get; set; }

    
    [ForeignKey(nameof(Base.AUTHORIZATIONS_BASE.AB_USER_REFNO))]
    // ReSharper disable once CollectionNeverUpdated.Global
    public ICollection<AUTHORIZATIONS_BASE> AUTHORIZATIONS_BASE { get; set; } = new HashSet<AUTHORIZATIONS_BASE>();
  
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
       UB_FULLNAME = $"{U_NAME} {U_SURNAME}";

       return new List<ValidationResult>();
    }
}