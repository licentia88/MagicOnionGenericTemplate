using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MemoryPack;

namespace MagicT.Server.Models;

[MemoryPackable]
public partial class UsersCredentials
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int UserId { get; set; }

    public string Identifier { get; set; }

    public byte[] SharedKey { get; set; }
}

 