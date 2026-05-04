using LobasOrdersApi.DTOs;
using LobasOrdersApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LobasOrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
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
        public IActionResult CreateProduct(ProductCreateDto productDto)
        {
            try
            {
                int newId = _productService.Create(productDto);

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
        public IActionResult UpdateProduct(int id, ProductUpdateDto productDto)
        {
            try
            {
                bool updated = _productService.Update(id, productDto);

                if (!updated)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                return Ok(new { message = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/stock")]
        public IActionResult UpdateStock(int id, [FromQuery] int stock)
        {
            try
            {
                bool updated = _productService.UpdateStock(id, stock);

                if (!updated)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                return Ok(new { message = "Stock actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            bool deleted = _productService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            return Ok(new { message = "Producto eliminado correctamente" });
        }
    }
}