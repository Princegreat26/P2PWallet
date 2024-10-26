using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P2PWallet.API.DTO;
using P2PWallet.Models.DTO;
using P2PWallet.Models.Entities;
using P2PWallet.Services;
using P2PWallet.Services.Interfaces;

namespace P2PWallet.API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDTO<List<User>>>> GetAllUsers()
        {
            var result = await _userServices.GettAllUsers();
            return Ok(new BaseResponseDTO<List<User>>
            {
                Status = true,
                StatusMessage = "Users retrieved successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseDTO<User>>> GetSingleUser(int id)
        {
            var result = await _userServices.GetSingleUser(id);
            if (result == null)
                return NotFound(new BaseResponseDTO<User>
                {
                    Status = false,
                    StatusMessage = "User doesn't have an account with us",
                    Data = null
                });

            return Ok(new BaseResponseDTO<User>
            {
                Status = true,
                StatusMessage = "User retrieved successfully",
                Data = result
            });
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUser(UserDTO user)
        {
            var result = await _userServices.CreateUser(user);
            return Ok(new BaseResponseDTO<bool>
            {
                Status = true,
                StatusMessage = "User created successfully",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponseDTO<List<User>>>> UpdateUser(int id, UpdateUserDTO updateUserDTO)
        {
            var result = await _userServices.UpdateUser(id, updateUserDTO);
            if (result == null)
                return NotFound(new BaseResponseDTO<List<User>>
                {
                    Status = false,
                    StatusMessage = "User doesn't have an account with us",
                    Data = null
                });

            return Ok(new BaseResponseDTO<List<User>>
            {
                Status = true,
                StatusMessage = "User updated successfully",
                Data = result
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponseDTO<List<User>>>> RemoveUser(int id)
        {
            var result = await _userServices.RemoveUser(id);
            if (result == null)
                return NotFound(new BaseResponseDTO<List<User>>
                {
                    Status = false,
                    StatusMessage = "User doesn't have an account with us",
                    Data = null
                });

            return Ok(new BaseResponseDTO<List<User>>
            {
                Status = true,
                StatusMessage = "User removed successfully",
                Data = result
            });
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseDTO<UserDTO>>> GetUserData()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var user = await _userServices.GetSingleUser(userId);
            if (user == null)
            {
                return NotFound(new BaseResponseDTO<UserDTO>
                {
                    Status = false,
                    StatusMessage = "User not found",
                    Data = null
                });
            }

            return Ok(new BaseResponseDTO<UserDTO>
            {
                Status = true,
                StatusMessage = "User data retrieved successfully",
                Data = user
            });
        }
    }
}
