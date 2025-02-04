using Coordinator.Models;

namespace Coordinator.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionParticipant _participantService;
    private readonly HttpClient _httpClient;

    public TransactionController(ILogger<TransactionController> logger, ITransactionParticipant participantService)
    {
        _logger = logger;
        _participantService = participantService;
        _httpClient = new HttpClient();
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartTransaction([FromBody] TransactionRequest request)
    {
        _logger.LogInformation($"Starting 2PC for transaction {request.TransactionId}");

        var prepareResults = new List<TransactionResponse>();

        foreach (var participantUrl in request.Participants)
        {
            var response = await _httpClient.PostAsJsonAsync($"{participantUrl}/api/transaction/prepare", request);
            var result = await response.Content.ReadFromJsonAsync<TransactionResponse>();

            if (result != null && result.Prepared)
            {
                prepareResults.Add(result);
            }
            else
            {
                _logger.LogError($"Prepare phase failed for {participantUrl}");
                await AbortTransaction(request.TransactionId, request.Participants);
                return BadRequest("Prepare phase failed. Rolling back...");
            }
        }

        // If all participants are prepared, commit
        foreach (var participantUrl in request.Participants)
        {
            var commitResponse = await _httpClient.PostAsJsonAsync($"{participantUrl}/api/transaction/commit", request);
            var commitResult = await commitResponse.Content.ReadFromJsonAsync<TransactionResponse>();

            if (commitResult == null || !commitResult.Committed)
            {
                _logger.LogError($"Commit phase failed for {participantUrl}");
                return BadRequest("Commit phase failed.");
            }
        }

        return Ok("Transaction committed successfully.");
    }

    [HttpPost("prepare")]
    public async Task<IActionResult> PrepareTransaction([FromBody] TransactionRequest request)
    {
        bool success = await _participantService.PrepareAsync(request.TransactionId, request.Data);
        return Ok(new TransactionResponse { Participant = "CurrentService", Prepared = success });
    }

    [HttpPost("commit")]
    public async Task<IActionResult> CommitTransaction([FromBody] TransactionRequest request)
    {
        bool success = await _participantService.CommitAsync(request.TransactionId);
        return Ok(new TransactionResponse { Participant = "CurrentService", Committed = success });
    }

    private async Task AbortTransaction(string transactionId, List<string> participants)
    {
        foreach (var participantUrl in participants)
        {
            await _httpClient.PostAsJsonAsync($"{participantUrl}/api/transaction/rollback", new { transactionId });
        }
    }
}
