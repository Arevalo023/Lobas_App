using LobasOrdersApi.Models;

namespace LobasOrdersApi.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();

        Customer? GetById(int id);

        List<Customer> Search(string searchTerm);

        int Create(Customer customer);

        bool Update(int id, Customer customer);

        bool Delete(int id);
    }
}