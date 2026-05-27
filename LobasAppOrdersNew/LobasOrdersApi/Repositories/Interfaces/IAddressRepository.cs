using LobasOrdersApi.Models;

namespace LobasOrdersApi.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        List<Address> GetAll();

        Address? GetById(int id);

        List<Address> GetByCustomerId(int customerId);

        int CountByCustomerId(int customerId);

        int Create(Address address);

        bool Update(int id, Address address);

        bool Delete(int id);

        bool ClearMainAddress(int customerId);
    }
}
