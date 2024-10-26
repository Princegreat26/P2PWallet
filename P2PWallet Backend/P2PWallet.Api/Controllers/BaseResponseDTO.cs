using P2PWallet.Models.Entities;

namespace P2PWallet.API.Controllers
{
    internal class BaseResponseDTO
    {
        public bool Status { get; set; }
        public string StatusMessage { get; set; }
        public List<Transaction> Data { get; set; }
    }
}