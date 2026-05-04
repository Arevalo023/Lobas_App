using LobasOrdersApi.DTOs;

namespace LobasOrdersApi.Services.Interfaces
{
    public interface ICustomerService
    {
        List<CustomerResponseDto> GetAll();

        CustomerResponseDto? GetById(int id);

        List<CustomerResponseDto> Search(string searchTerm);

        int Create(CustomerCreateDto customerDto);

        bool Update(int id, CustomerUpdateDto customerDto);

        bool Delete(int id);
    }
}