 using MasterMemory;
 using MessagePack;

 namespace MagicT.Shared.Models.MemoryDatabaseModels;

 [MemoryTable(nameof(Authorizations)), MessagePackObject(true)]
 public partial class Authorizations
 {
     [PrimaryKey]
     public virtual int Id { get; set; }

     [SecondaryKey(0, 1)]
     public virtual int UserRefNo { get; set; }

     [SecondaryKey(1, 1)]
     public virtual string AuthType { get; set; }

     [IgnoreMember]
     public string Description { get; set; }
 }

