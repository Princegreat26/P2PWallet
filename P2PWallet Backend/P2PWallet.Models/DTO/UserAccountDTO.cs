using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PWallet.Models.DTO
{
    public class UserAccountDTO
    {
        public string AccountNumber { get; set; }

        public decimal Balance { get; set; } = 10000m;

        public string Currency { get; set; }
    }
}
