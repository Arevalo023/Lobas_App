using LobasOrdersApi.DTOs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailsController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public IActionResult GetOrderDetails()
        {
            List<OrderDetailResponseDto> details = _orderDetailService.GetAll();

            return Ok(details);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderDetailById(int id)
        {
            OrderDetailResponseDto? detail = _orderDetailService.GetById(id);

            if (detail == null)
            {
                return NotFound(new { message = "Detalle de pedido no encontrado" });
            }

            return Ok(detail);
        }

        [HttpGet("order/{orderId}")]
        public IActionResult GetDetailsByOrderId(int orderId)
        {
            List<OrderDetailResponseDto> details = _orderDetailService.GetByOrderId(orderId);

            return Ok(details);
        }

        [HttpPost]
        public IActionResult CreateOrderDetail(OrderDetailCreateStandaloneDto detailDto)
        {
            try
            {
                int newId = _orderDetailService.Create(detailDto);

                return Ok(new
                {
                    message = "Detalle de pedido creado correctamente",
                    id = newId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateOrderDetail(int id, OrderDetailUpdateDto detailDto)
        {
            try
            {
                bool updated = _orderDetailService.Update(id, detailDto);

                if (!updated)
                {
                    return NotFound(new { message = "Detalle de pedido no encontrado" });
                }

                return Ok(new { message = "Detalle de pedido actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            bool deleted = _orderDetailService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Detalle de pedido no encontrado" });
            }

            return Ok(new { message = "Detalle de pedido eliminado correctamente" });
        }
    }
}