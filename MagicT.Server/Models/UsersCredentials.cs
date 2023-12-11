using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MemoryPack;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Models;

[MemoryPackable]
[Index(nameof(UserId))]
public partial class UsersCredentials
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    public int UserId { get; set; }

    public string Identifier { get; set; }

    public byte[] SharedKey { get; set; }

    public DateTime Date { get; set; }
}

 