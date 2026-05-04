using LobasOrdersApi.DTOs;
using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using LobasOrdersApi.Services.Interfaces;

namespace LobasOrdersApi.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICustomerRepository _customerRepository;

        public AddressService(
            IAddressRepository addressRepository,
            ICustomerRepository customerRepository)
        {
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
        }

        public List<AddressResponseDto> GetAll()
        {
            List<Address> addresses = _addressRepository.GetAll();

            return addresses.Select(MapToResponseDto).ToList();
        }

        public AddressResponseDto? GetById(int id)
        {
            Address? address = _addressRepository.GetById(id);

            if (address == null)
            {
                return null;
            }

            return MapToResponseDto(address);
        }

        public List<AddressResponseDto> GetByCustomerId(int customerId)
        {
            List<Address> addresses = _addressRepository.GetByCustomerId(customerId);

            return addresses.Select(MapToResponseDto).ToList();
        }

        public int Create(AddressCreateDto addressDto)
        {
            ValidateAddress(
                addressDto.CustomerId,
                addressDto.Street,
                addressDto.City,
                addressDto.State,
                addressDto.ZipCode
            );

            if (addressDto.IsMain)
            {
                _addressRepository.ClearMainAddress(addressDto.CustomerId);
            }

            Address address = new Address
            {
                CustomerId = addressDto.CustomerId,
                Street = addressDto.Street.Trim(),
                City = addressDto.City.Trim(),
                State = addressDto.State.Trim(),
                ZipCode = addressDto.ZipCode.Trim(),
                IsMain = addressDto.IsMain
            };

            return _addressRepository.Create(address);
        }

        public bool Update(int id, AddressUpdateDto addressDto)
        {
            Address? existingAddress = _addressRepository.GetById(id);

            if (existingAddress == null)
            {
                return false;
            }

            ValidateAddress(
                existingAddress.CustomerId,
                addressDto.Street,
                addressDto.City,
                addressDto.State,
                addressDto.ZipCode
            );

            if (addressDto.IsMain)
            {
                _addressRepository.ClearMainAddress(existingAddress.CustomerId);
            }

            Address address = new Address
            {
                CustomerId = existingAddress.CustomerId,
                Street = addressDto.Street.Trim(),
                City = addressDto.City.Trim(),
                State = addressDto.State.Trim(),
                ZipCode = addressDto.ZipCode.Trim(),
                IsMain = addressDto.IsMain
            };

            return _addressRepository.Update(id, address);
        }

        public bool Delete(int id)
        {
            return _addressRepository.Delete(id);
        }

        private void ValidateAddress(
            int customerId,
            string street,
            string city,
            string state,
            string zipCode)
        {
            Customer? customer = _customerRepository.GetById(customerId);

            if (customer == null)
            {
                throw new Exception("El cliente no existe");
            }

            if (string.IsNullOrWhiteSpace(street))
            {
                throw new Exception("La calle es obligatoria");
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new Exception("La ciudad es obligatoria");
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                throw new Exception("El estado es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(zipCode))
            {
                throw new Exception("El código postal es obligatorio");
            }
        }

        private AddressResponseDto MapToResponseDto(Address address)
        {
            Customer? customer = _customerRepository.GetById(address.CustomerId);

            return new AddressResponseDto
            {
                Id = address.Id,
                CustomerId = address.CustomerId,
                CustomerName = customer?.Name ?? "Cliente no encontrado",
                Street = address.Street,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode,
                IsMain = address.IsMain
            };
        }
    }
}