using System.ComponentModel.DataAnnotations.Schema;
using MagicT.Shared.Models.Base;

namespace MagicT.Shared.Models;



[Equatable]
[MemoryPackable]
[GenerateDataReaderMapper]
[Table(nameof(PERMISSIONS))]
// ReSharper disable once PartialTypeWithSinglePart
public sealed partial class PERMISSIONS : AUTHORIZATIONS_BASE ,IAUTHORIZATIONS_BASE
{
    public PERMISSIONS() =>AB_AUTH_TYPE = nameof(PERMISSIONS);

    public int PER_ROLE_REFNO { get; set; }

    public string PER_PERMISSION_NAME { get; set; }

 }

 