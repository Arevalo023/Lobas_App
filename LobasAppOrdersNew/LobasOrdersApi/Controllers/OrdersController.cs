using LobasOrdersApi.DTOs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
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
        public IActionResult CreateOrder(OrderCreateDto orderDto)
        {
            try
            {
                int newId = _orderService.Create(orderDto);

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
        public IActionResult UpdateOrder(int id, OrderUpdateDto orderDto)
        {
            try
            {
                bool updated = _orderService.Update(id, orderDto);

                if (!updated)
                {
                    return NotFound(new { message = "Pedido no encontrado" });
                }

                return Ok(new { message = "Pedido actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public IActionResult UpdateStatus(int id, OrderStatusUpdateDto statusDto)
        {
            try
            {
                bool updated = _orderService.UpdateStatus(id, statusDto);

                if (!updated)
                {
                    return NotFound(new { message = "Pedido no encontrado" });
                }

                return Ok(new { message = "Estatus actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            bool deleted = _orderService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            return Ok(new { message = "Pedido eliminado correctamente" });
        }
    }
}