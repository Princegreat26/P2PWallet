using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2PWallet.Models.DTO
{
    public class UserDTO
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public List<Account> Accounts { get; set; }

        public string Email { get; set; } = string.Empty;
    }

     public class Account
    {
        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public string Currency {  get; set; }
    }
}
