using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2PWallet.API.Data;
using P2PWallet.Models.DTO;
using P2PWallet.Models.Entities;
using P2PWallet.Services.Interfaces;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    private readonly P2PDataContext _context;

    private readonly ILogger<TransactionController> _logger;

    public TransactionController(ITransactionService transactionService, P2PDataContext context, ILogger<TransactionController> logger)
    {
        _context = context;
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions()
    {
        var userId = User.FindFirst("UserId")?.Value;
        //if (userId == null)
        //{
        //    return Unauthorized(new BaseResponseDTO<object>
        //    {
        //        Status = false,
        //        StatusMessage = "Unauthorized",
        //        Data = null
        //    });


        var transactions = await _transactionService.GetAllTransactions(userId);
        if (transactions == null)
        {
            return NotFound(new BaseResponseDTO<object>
            {
                Status = false,
                StatusMessage = "Transactions not found",
                Data = null
            });
        }

        return Ok(new BaseResponseDTO<object>
        {
            Status = true,
            StatusMessage = "Transactions retrieved successfully",
            Data = transactions
        });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferMoney([FromBody] TransferRequest request)
    {
        var userId = User.FindFirst("UserId")?.Value;

        // Log the value of userId
        _logger.LogInformation($"User ID: {userId}");

        var user = await _context.Users.Include(x => x.UserAccount).FirstOrDefaultAsync(x => Convert.ToString(x.Id) == userId);
        if (user == null)
        {
            return NotFound(new BaseResponseDTO<object>
            {
                Status = false,
                StatusMessage = "User not found",
                Data = null
            });
        }

        var sourceAccount = user.UserAccount.FirstOrDefault(x => x.Currency == "NGN");
        if (sourceAccount == null)
        {
            return BadRequest(new BaseResponseDTO<object>
            {
                Status = false,
                StatusMessage = "Source account not found",
                Data = null
            });
        }

        var result = await _transactionService.TransferMoney(sourceAccount.AccountNumber, request.DestinationAccountNumber, request.Amount);
        if (result)
        {
            return Ok(new BaseResponseDTO<object>
            {
                Status = true,
                StatusMessage = "Transfer successful",
                Data = null
            });
        }
        else
        {
            return BadRequest(new BaseResponseDTO<object>
            {
                Status = false,
                StatusMessage = "Transfer failed",
                Data = null
            });
        }
    }

    
    [HttpGet("account")]
    public async Task<IActionResult> GetUserAccountNumber()
    {
        var userId = User.FindFirst("UserId")?.Value;
        //if (userId == null)
        //{
        //    return Unauthorized(new BaseResponseDTO<object>
        //    {
        //        Status = false,
        //        StatusMessage = "Unauthorized",
        //        Data = null
        //    });
        //}

        // Add logging to check the value of the user ID
        _logger.LogInformation($"User ID: {userId}");

        var user = await _context.Users.Include(x => x.UserAccount).FirstOrDefaultAsync(x => Convert.ToString(x.Id) == userId);
        if (user == null || user.UserAccount.Count == 0)
        {
            return NotFound(new BaseResponseDTO<object>
            {
                Status = false,
                StatusMessage = "User account not found",
                Data = null
            });
        }

        var accountNumber = user.UserAccount.FirstOrDefault(x => x.Currency == "NGN")?.AccountNumber;
        if (accountNumber == null)
        {
            return BadRequest(new BaseResponseDTO<object>
            {
                Status = false,
                StatusMessage = "Source account not found",
                Data = null
            });
        }

        return Ok(new BaseResponseDTO<object>
        {
            Status = true,
            StatusMessage = "Account number retrieved successfully",
            Data = new { accountNumber }
        });
    }


    public class TransferRequest
    {
        //public string SourceAccountNumber { get; set; }

        public string DestinationAccountNumber { get; set; }

        public decimal Amount { get; set; }
    }
}