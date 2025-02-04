using Coordinator.Models;

namespace Service1.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionParticipant _participantService;

    public TransactionController(ITransactionParticipant participantService)
    {
        _participantService = participantService;
    }

    [HttpPost("prepare")]
    public async Task<IActionResult> PrepareTransaction([FromBody] TransactionRequest request)
    {
        bool success = await _participantService.PrepareAsync(request.TransactionId, request.Data);
        return Ok(new TransactionResponse { Participant = "Service1", Prepared = success });
    }

    [HttpPost("commit")]
    public async Task<IActionResult> CommitTransaction([FromBody] TransactionRequest request)
    {
        bool success = await _participantService.CommitAsync(request.TransactionId);
        return Ok(new TransactionResponse { Participant = "Service1", Committed = success });
    }

    [HttpPost("rollback")]
    public async Task<IActionResult> RollbackTransaction([FromBody] TransactionRequest request)
    {
        bool success = await _participantService.RollbackAsync(request.TransactionId);
        return Ok(new { Message = success ? "Rolled back successfully" : "Rollback failed" });
    }
}
