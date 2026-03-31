using BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;

namespace WebApiLayer.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            try
            {
                var result = _userService.Register(dto);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            try
            {
                var result = _userService.Login(dto);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        // POST /api/auth/google
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin(GoogleAuthDto dto)
        {
            try
            {
                var result = await _userService.GoogleLoginAsync(dto);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
