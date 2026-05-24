using LobasOrdersApi.DTOs;
using LobasOrdersApi.Hubs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IHubContext<AppNotificationsHub> _hubContext;

        public CustomersController(
            ICustomerService customerService,
            IHubContext<AppNotificationsHub> hubContext)
        {
            _customerService = customerService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            List<CustomerResponseDto> customers = _customerService.GetAll();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            CustomerResponseDto? customer = _customerService.GetById(id);

            if (customer == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }

            return Ok(customer);
        }

        [HttpGet("search")]
        public IActionResult SearchCustomers([FromQuery] string searchTerm)
        {
            List<CustomerResponseDto> customers = _customerService.Search(searchTerm);

            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerCreateDto customerDto)
        {
            try
            {
                int newId = _customerService.Create(customerDto);

                await _hubContext.Clients.All.SendAsync("CustomersChanged");

                return Ok(new
                {
                    message = "Cliente creado correctamente",
                    id = newId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerUpdateDto customerDto)
        {
            try
            {
                bool updated = _customerService.Update(id, customerDto);

                if (!updated)
                {
                    return NotFound(new { message = "Cliente no encontrado" });
                }

                await _hubContext.Clients.All.SendAsync("CustomersChanged");

                return Ok(new { message = "Cliente actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                bool deleted = _customerService.Delete(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Cliente no encontrado" });
                }

                await _hubContext.Clients.All.SendAsync("CustomersChanged");

                return Ok(new { message = "Cliente eliminado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
