using LobasOrdersApi.DTOs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserCreateDto userDto)
        {
            try
            {
                UserResponseDto user = _authService.Register(userDto);

                return Ok(new
                {
                    message = "Usuario registrado correctamente",
                    user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto loginDto)
        {
            try
            {
                UserResponseDto user = _authService.Login(loginDto);

                return Ok(new
                {
                    message = "Inicio de sesión correcto",
                    user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("social-login")]
        public IActionResult SocialLogin(SocialLoginDto socialLoginDto)
        {
            try
            {
                UserResponseDto user = _authService.SocialLogin(socialLoginDto);

                return Ok(new
                {
                    message = "Inicio de sesión social correcto",
                    user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}