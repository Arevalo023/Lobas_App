using LobasOrdersApi.DTOs;
using LobasOrdersApi.Hubs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHubContext<AppNotificationsHub> _hubContext;

        public OrdersController(
            IOrderService orderService,
            IHubContext<AppNotificationsHub> hubContext)
        {
            _orderService = orderService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            List<OrderResponseDto> orders = _orderService.GetAll();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            OrderResponseDto? order = _orderService.GetById(id);

            if (order == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            return Ok(order);
        }

        [HttpGet("customer/{customerId}")]
        public IActionResult GetOrdersByCustomerId(int customerId)
        {
            List<OrderResponseDto> orders = _orderService.GetByCustomerId(customerId);

            return Ok(orders);
        }

        [HttpGet("search")]
        public IActionResult SearchOrders([FromQuery] string searchTerm)
        {
            List<OrderResponseDto> orders = _orderService.Search(searchTerm);

            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDto orderDto)
        {
            try
            {
                int newId = _orderService.Create(orderDto);

                await _hubContext.Clients.All.SendAsync("OrdersChanged");
                await _hubContext.Clients.All.SendAsync("ProductsChanged");

                return Ok(new
                {
                    message = "Pedido creado correctamente",
                    id = newId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDto orderDto)
        {
            try
            {
                bool updated = _orderService.Update(id, orderDto);

                if (!updated)
                {
                    return NotFound(new { message = "Pedido no encontrado" });
                }

                await _hubContext.Clients.All.SendAsync("OrdersChanged");

                return Ok(new { message = "Pedido actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatusUpdateDto statusDto)
        {
            try
            {
                bool updated = _orderService.UpdateStatus(id, statusDto);

                if (!updated)
                {
                    return NotFound(new { message = "Pedido no encontrado" });
                }

                await _hubContext.Clients.All.SendAsync("OrdersChanged");

                return Ok(new { message = "Estatus actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            bool deleted = _orderService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            await _hubContext.Clients.All.SendAsync("OrdersChanged");

            return Ok(new { message = "Pedido eliminado correctamente" });
        }
    }
}
