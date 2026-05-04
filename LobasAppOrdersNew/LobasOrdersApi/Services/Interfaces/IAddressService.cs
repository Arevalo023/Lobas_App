using LobasOrdersApi.DTOs;

namespace LobasOrdersApi.Services.Interfaces
{
    public interface IAddressService
    {
        List<AddressResponseDto> GetAll();

        AddressResponseDto? GetById(int id);

        List<AddressResponseDto> GetByCustomerId(int customerId);

        int Create(AddressCreateDto addressDto);

        bool Update(int id, AddressUpdateDto addressDto);

        bool Delete(int id);
    }
}