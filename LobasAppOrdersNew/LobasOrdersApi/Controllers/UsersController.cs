using LobasOrdersApi.DTOs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            List<UserResponseDto> users = _userService.GetAll();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            UserResponseDto? user = _userService.GetById(id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserUpdateDto userDto)
        {
            try
            {
                bool updated = _userService.Update(id, userDto);

                if (!updated)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(new { message = "Usuario actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/biometric")]
        public IActionResult UpdateBiometricStatus(int id, [FromQuery] bool biometricEnabled)
        {
            bool updated = _userService.UpdateBiometricStatus(id, biometricEnabled);

            if (!updated)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Estado biométrico actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            bool deleted = _userService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Usuario eliminado correctamente" });
        }
    }
}