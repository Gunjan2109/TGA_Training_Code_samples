namespace Coordinator.Models;

public interface ITransactionParticipant
{
    Task<bool> PrepareAsync(string transactionId, Dictionary<string, object> data);
    Task<bool> CommitAsync(string transactionId);
    Task<bool> RollbackAsync(string transactionId);
}