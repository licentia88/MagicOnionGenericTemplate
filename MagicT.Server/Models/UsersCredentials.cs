namespace MagicT.Server.Models;

[global::MemoryPack.MemoryPackable]
public partial class UsersCredentials
{
    [global::System.ComponentModel.DataAnnotations.Key, global::System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(global::System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
    public int UserId { get; set; }

    public string Identifier { get; set; }

    public byte[] SharedKey { get; set; }
}

 