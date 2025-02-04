namespace Coordinator.Models;

public class TransactionResponse
{
    public string Participant { get; set; }
    public bool Prepared { get; set; }
    public bool Committed { get; set; }
}