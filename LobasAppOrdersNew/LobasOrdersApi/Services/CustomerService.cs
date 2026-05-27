using LobasOrdersApi.DTOs;
using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using LobasOrdersApi.Services.Interfaces;

namespace LobasOrdersApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAddressRepository _addressRepository;

        public CustomerService(
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IAddressRepository addressRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _addressRepository = addressRepository;
        }

        public List<CustomerResponseDto> GetAll()
        {
            List<Customer> customers = _customerRepository.GetAll();

            return customers.Select(MapToResponseDto).ToList();
        }

        public CustomerResponseDto? GetById(int id)
        {
            Customer? customer = _customerRepository.GetById(id);

            if (customer == null)
            {
                return null;
            }

            return MapToResponseDto(customer);
        }

        public List<CustomerResponseDto> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAll();
            }

            List<Customer> customers = _customerRepository.Search(searchTerm.Trim());

            return customers.Select(MapToResponseDto).ToList();
        }

        public int Create(CustomerCreateDto customerDto)
        {
            ValidateCustomer(customerDto.Name);

            Customer customer = new Customer
            {
                Name = customerDto.Name.Trim(),
                Email = string.IsNullOrWhiteSpace(customerDto.Email) ? null : customerDto.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(customerDto.Phone) ? null : customerDto.Phone.Trim()
            };

            return _customerRepository.Create(customer);
        }

        public bool Update(int id, CustomerUpdateDto customerDto)
        {
            ValidateCustomer(customerDto.Name);

            Customer customer = new Customer
            {
                Name = customerDto.Name.Trim(),
                Email = string.IsNullOrWhiteSpace(customerDto.Email) ? null : customerDto.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(customerDto.Phone) ? null : customerDto.Phone.Trim()
            };

            return _customerRepository.Update(id, customer);
        }

        public bool Delete(int id)
        {
            List<Order> customerOrders = _orderRepository.GetByCustomerId(id);

            if (customerOrders.Count > 0)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el cliente porque tiene pedidos registrados. Elimina o reasigna esos pedidos antes de eliminar el cliente.");
            }

            return _customerRepository.Delete(id);
        }

        private void ValidateCustomer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("El nombre del cliente es obligatorio");
            }
        }

        private CustomerResponseDto MapToResponseDto(Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedAt = customer.CreatedAt,
                IsActive = customer.IsActive,
                AddressCount = _addressRepository.CountByCustomerId(customer.Id)
            };
        }
    }
}
