using Microsoft.EntityFrameworkCore;
using P2PWallet.API.Data;
using P2PWallet.Models.Entities;
using P2PWallet.Services.Interfaces;

public class TransactionService : ITransactionService
{
    private readonly P2PDataContext _context;

    public TransactionService(P2PDataContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetAllTransactions(string userId)
    {
        try
        {
            var user = await _context.Users.Include(x => x.UserAccount).FirstOrDefaultAsync(x => Convert.ToString(x.Id) == userId);
            if (user == null || user.UserAccount.Count == 0) return null;
            var acctNum = user.UserAccount.FirstOrDefault(x => x.Currency == "NGN")?.AccountNumber;
            if (acctNum == null) return null;
            var transfers = await _context.Transactions.Where(x => x.SourceAccountNumber == acctNum || x.DestinationAccountNumber == acctNum).ToListAsync();
            return transfers;
        }
        catch (Exception ex)
        {
            // Log the exception
            throw;
        }
    }

    public async Task<bool> TransferMoney(string sourceAccountNumber, string destinationAccountNumber, decimal amount)
    {
        try
        {
            var sourceAccount = await _context.UserAccounts.FirstOrDefaultAsync(x => x.AccountNumber == sourceAccountNumber);
            var destinationAccount = await _context.UserAccounts.FirstOrDefaultAsync(x => x.AccountNumber == destinationAccountNumber);

            if (sourceAccount == null || destinationAccount == null)
            {
                return false; // Account not found
            }

            if (amount <= 0)
            {
                return false; // Amount must be positive
            }

            if (sourceAccount.Balance < amount)
            {
                return false; // Insufficient funds
            }

            // Deduct amount from source account
            sourceAccount.Balance -= amount;

            // Add amount to destination account
            destinationAccount.Balance += amount;

            // Create a new transaction record
            var transaction = new Transaction
            {
                Amount = amount,
                Date = DateTime.UtcNow,
                Status = "Completed",
                SourceAccountNumber = sourceAccountNumber,
                DestinationAccountNumber = destinationAccountNumber,
                TransactionType = "Transfer",
                Reference = Guid.NewGuid().ToString(),
                UserAccountId = sourceAccount.Id // Assuming UserAccountId is the ID of the source account
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            throw;
        }
    }
}
