using LobasOrdersApi.DTOs;

namespace LobasOrdersApi.Services.Interfaces
{
    public interface IOrderService
    {
        List<OrderResponseDto> GetAll();

        OrderResponseDto? GetById(int id);

        List<OrderResponseDto> GetByCustomerId(int customerId);

        List<OrderResponseDto> Search(string searchTerm);

        int Create(OrderCreateDto orderDto);

        bool Update(int id, OrderUpdateDto orderDto);

        bool UpdateStatus(int id, OrderStatusUpdateDto statusDto);

        bool Delete(int id);
    }
}