namespace Coordinator.Models;

public class TransactionRequest
{
    public string TransactionId { get; set; }
    public List<string> Participants { get; set; }  // Services involved
    public Dictionary<string, object> Data { get; set; }
}