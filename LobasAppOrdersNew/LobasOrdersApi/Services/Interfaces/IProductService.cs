using LobasOrdersApi.DTOs;

namespace LobasOrdersApi.Services.Interfaces
{
    public interface IProductService
    {
        List<ProductResponseDto> GetAll();

        ProductResponseDto? GetById(int id);

        List<ProductResponseDto> Search(string searchTerm);

        int Create(ProductCreateDto productDto);

        bool Update(int id, ProductUpdateDto productDto);

        bool Delete(int id);

        bool UpdateStock(int productId, int newStock);
    }
}