using Coordinator.Models;

namespace Coordinator.Services;

public class TransactionParticipantService : ITransactionParticipant
{
    private readonly ILogger<TransactionParticipantService> _logger;
    
    public TransactionParticipantService(ILogger<TransactionParticipantService> logger)
    {
        _logger = logger;
    }

    private static readonly Dictionary<string, Dictionary<string, object>> _transactionStore = new();

    public async Task<bool> PrepareAsync(string transactionId, Dictionary<string, object> data)
    {
        _logger.LogInformation($"Preparing transaction: {transactionId}");

        if (_transactionStore.ContainsKey(transactionId))
            return false;

        _transactionStore[transactionId] = data;
        return await Task.FromResult(true);
    }

    public async Task<bool> CommitAsync(string transactionId)
    {
        _logger.LogInformation($"Committing transaction: {transactionId}");

        if (!_transactionStore.ContainsKey(transactionId))
            return false;

        _transactionStore.Remove(transactionId);
        return await Task.FromResult(false);
    }

    public async Task<bool> RollbackAsync(string transactionId)
    {
        _logger.LogInformation($"Rolling back transaction: {transactionId}");

        if (_transactionStore.ContainsKey(transactionId))
            _transactionStore.Remove(transactionId);

        return await Task.FromResult(true);
    }
}
