using P2PWallet.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PWallet.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllTransactions(string userId);

        Task<bool> TransferMoney(string sourceAccountNumber, string destinationAccountNumber, decimal amount);
    }
}
