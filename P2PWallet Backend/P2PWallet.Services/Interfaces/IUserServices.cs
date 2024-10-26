using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P2PWallet.API.DTO;
using P2PWallet.Models;
using P2PWallet.Models.DTO;

namespace P2PWallet.Services.Interfaces
{
    public interface IUserServices
    {
        Task<List<UserDTO>> GettAllUsers();

        Task<UserDTO> GetSingleUser(int id);

        Task<bool>? CreateUser(UserDTO user);

        Task<bool> UpdateUser(int id, UpdateUserDTO updateUserDTO);

        Task<bool>? RemoveUser(int id);

        Task<bool> SignUp(SignUpDTO request);

        Task<string> Login(LoginDTO request);
    }
}
