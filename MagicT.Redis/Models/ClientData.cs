using MemoryPack;

namespace MagicT.Redis.Models;

 
[MemoryPackable]

public partial class ClientData
{
   
    public ClientData()
    {
        
    }
    
    [MemoryPackConstructor]
    public ClientData(string ip, string status, int? duration)
    {
        Ip = ip;
        Status = status;
        Duration = duration;
    }

    public string Ip { get; set; }
    public string Status { get; set; }
    public int? Duration { get; set; }
}