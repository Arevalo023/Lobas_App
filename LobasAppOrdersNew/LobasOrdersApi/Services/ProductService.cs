using LobasOrdersApi.DTOs;
using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using LobasOrdersApi.Services.Interfaces;

namespace LobasOrdersApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<ProductResponseDto> GetAll()
        {
            List<Product> products = _productRepository.GetAll();

            return products.Select(MapToResponseDto).ToList();
        }

        public ProductResponseDto? GetById(int id)
        {
            Product? product = _productRepository.GetById(id);

            if (product == null)
            {
                return null;
            }

            return MapToResponseDto(product);
        }

        public List<ProductResponseDto> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAll();
            }

            List<Product> products = _productRepository.Search(searchTerm.Trim());

            return products.Select(MapToResponseDto).ToList();
        }

        public int Create(ProductCreateDto productDto)
        {
            ValidateProduct(productDto.Name, productDto.Price, productDto.Stock);

            Product product = new Product
            {
                Name = productDto.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(productDto.Description)
                    ? null
                    : productDto.Description.Trim(),
                Price = productDto.Price,
                Stock = productDto.Stock
            };

            return _productRepository.Create(product);
        }

        public bool Update(int id, ProductUpdateDto productDto)
        {
            ValidateProduct(productDto.Name, productDto.Price, productDto.Stock);

            Product product = new Product
            {
                Name = productDto.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(productDto.Description)
                    ? null
                    : productDto.Description.Trim(),
                Price = productDto.Price,
                Stock = productDto.Stock
            };

            return _productRepository.Update(id, product);
        }

        public bool Delete(int id)
        {
            return _productRepository.Delete(id);
        }

        public bool UpdateStock(int productId, int newStock)
        {
            if (newStock < 0)
            {
                throw new Exception("El stock no puede ser negativo");
            }

            return _productRepository.UpdateStock(productId, newStock);
        }

        private void ValidateProduct(string name, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("El nombre del producto es obligatorio");
            }

            if (price < 0)
            {
                throw new Exception("El precio no puede ser negativo");
            }

            if (stock < 0)
            {
                throw new Exception("El stock no puede ser negativo");
            }
        }

        private ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        }
    }
}