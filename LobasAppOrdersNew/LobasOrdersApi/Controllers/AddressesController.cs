using LobasOrdersApi.DTOs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        public IActionResult GetAddresses()
        {
            List<AddressResponseDto> addresses = _addressService.GetAll();

            return Ok(addresses);
        }

        [HttpGet("{id}")]
        public IActionResult GetAddressById(int id)
        {
            AddressResponseDto? address = _addressService.GetById(id);

            if (address == null)
            {
                return NotFound(new { message = "Dirección no encontrada" });
            }

            return Ok(address);
        }

        [HttpGet("customer/{customerId}")]
        public IActionResult GetAddressesByCustomerId(int customerId)
        {
            List<AddressResponseDto> addresses = _addressService.GetByCustomerId(customerId);

            return Ok(addresses);
        }

        [HttpPost]
        public IActionResult CreateAddress(AddressCreateDto addressDto)
        {
            try
            {
                int newId = _addressService.Create(addressDto);

                return Ok(new
                {
                    message = "Dirección creada correctamente",
                    id = newId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAddress(int id, AddressUpdateDto addressDto)
        {
            try
            {
                bool updated = _addressService.Update(id, addressDto);

                if (!updated)
                {
                    return NotFound(new { message = "Dirección no encontrada" });
                }

                return Ok(new { message = "Dirección actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAddress(int id)
        {
            bool deleted = _addressService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Dirección no encontrada" });
            }

            return Ok(new { message = "Dirección eliminada correctamente" });
        }
    }
}