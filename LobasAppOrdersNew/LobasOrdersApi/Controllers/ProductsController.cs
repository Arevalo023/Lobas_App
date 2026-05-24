using LobasOrdersApi.DTOs;
using LobasOrdersApi.Hubs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IHubContext<AppNotificationsHub> _hubContext;

        public ProductsController(
            IProductService productService,
            IHubContext<AppNotificationsHub> hubContext)
        {
            _productService = productService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            List<ProductResponseDto> products = _productService.GetAll();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            ProductResponseDto? product = _productService.GetById(id);

            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            return Ok(product);
        }

        [HttpGet("search")]
        public IActionResult SearchProducts([FromQuery] string searchTerm)
        {
            List<ProductResponseDto> products = _productService.Search(searchTerm);

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto productDto)
        {
            try
            {
                int newId = _productService.Create(productDto);

                await _hubContext.Clients.All.SendAsync("ProductsChanged");

                return Ok(new
                {
                    message = "Producto creado correctamente",
                    id = newId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto productDto)
        {
            try
            {
                bool updated = _productService.Update(id, productDto);

                if (!updated)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                await _hubContext.Clients.All.SendAsync("ProductsChanged");

                return Ok(new { message = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromQuery] int stock)
        {
            try
            {
                bool updated = _productService.UpdateStock(id, stock);

                if (!updated)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                await _hubContext.Clients.All.SendAsync("ProductsChanged");

                return Ok(new { message = "Stock actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            bool deleted = _productService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            await _hubContext.Clients.All.SendAsync("ProductsChanged");

            return Ok(new { message = "Producto eliminado correctamente" });
        }
    }
}
