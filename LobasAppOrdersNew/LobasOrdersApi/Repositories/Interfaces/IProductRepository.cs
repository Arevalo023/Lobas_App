using LobasOrdersApi.Models;

namespace LobasOrdersApi.Repositories.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAll();

        Product? GetById(int id);

        List<Product> Search(string searchTerm);

        int Create(Product product);

        bool Update(int id, Product product);

        bool Delete(int id);

        bool UpdateStock(int productId, int newStock);
    }
}