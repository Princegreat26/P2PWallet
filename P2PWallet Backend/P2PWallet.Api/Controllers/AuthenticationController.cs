using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P2PWallet.API.DTO;
using P2PWallet.Models.DTO;
using P2PWallet.Models.Entities;
using P2PWallet.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace P2PWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public static User user = new User();

        private readonly IUserServices _userServices;

        public AuthenticationController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("signup")]
        [EnableCors("AllowSpecificOrigin")] // added this one
        public async Task<ActionResult<bool>> SignUp(SignUpDTO request)
        {
            var result = await _userServices.SignUp(request);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<BaseResponseDTO<string>>> Login(LoginDTO request)
        {
            var token = await _userServices.Login(request);
            if (token.StartsWith("Incorrect"))
            {
                return Unauthorized(new BaseResponseDTO<string>
                {
                    Status = false,
                    StatusMessage = token,
                    Data = null
                });
            }

            return Ok(new BaseResponseDTO<string>
            {
                Status = true,
                StatusMessage = "Login successful",
                Data = token
            });
        }

        [Authorize]
        [HttpGet("dashboard")]
        public async Task<ActionResult> Dashboard()
        {
            string name = User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            return Ok($"Authenticated {name}");
        }
    }
}

