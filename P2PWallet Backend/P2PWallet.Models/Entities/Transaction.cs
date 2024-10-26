using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PWallet.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; } = string.Empty;

        public string SourceAccountNumber { get; set; } = string.Empty;

        public string DestinationAccountNumber { get; set; } = string.Empty;

        public string TransactionType { get; set; } = string.Empty;

        public string Reference { get; set; } = string.Empty;

        // Foreign key to UserAccount
        [ForeignKey("UserAccount")]
        public int UserAccountId { get; set; }

        public virtual UserAccount UserAccount { get; set; }
    }
}
