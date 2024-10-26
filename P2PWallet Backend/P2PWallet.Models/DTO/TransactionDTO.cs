using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PWallet.Models.DTO
{
    public class TransactionDTO
    {
        [Required]
        public string Pin { get; set; }

        [Required]
        public string DestinationAccountNumber { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
