namespace Infrastructure.Models;

public class KeysGift
{
    public uint Id { get; set; }
    public ulong ServerId { get; set; }
    public string Gift { get; set; }
    
    public ulong AddedAt { get; set; }
    public ulong AddedBy { get; set; }
    public ulong ActivationAt { get; set; }
    public ulong ActivationBy { get; set; }
}