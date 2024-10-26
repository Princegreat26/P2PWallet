using P2PWallet.Models;
using P2PWallet.API;
using P2PWallet.Models.DTO;

namespace P2PWallet.API.DTO
{
    public class SignUpDTO
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber {  get; set; } = string.Empty;
    }
}
