using LobasOrdersApi.Models;

namespace LobasOrdersApi.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        List<Order> GetAll();

        Order? GetById(int id);

        List<Order> GetByCustomerId(int customerId);

        List<Order> Search(string searchTerm);

        int Create(Order order);

        bool Update(int id, Order order);

        bool UpdateStatus(int id, string status);

        bool UpdateTotal(int id, decimal total);

        bool Delete(int id);
    }
}