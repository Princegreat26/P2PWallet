using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P2PWallet.Models;

namespace P2PWallet.Models.DTO
{
    public class BaseResponseDTO<T>
    {
        public bool Status { get; set; }

        public string StatusMessage { get; set; }

        public object Data { get; set; }
    }
}
