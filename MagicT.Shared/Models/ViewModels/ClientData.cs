namespace MagicT.Shared.Models.ViewModels;

[MemoryPackable]
public partial class ClientData
{
    public string Id { get; set; }
    public string Ip { get; set; }
    public string Status { get; set; }
    public TimeSpan Duration { get; set; }
}