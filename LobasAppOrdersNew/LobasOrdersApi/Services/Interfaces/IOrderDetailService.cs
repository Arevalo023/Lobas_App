using LobasOrdersApi.DTOs;

namespace LobasOrdersApi.Services.Interfaces
{
    public interface IOrderDetailService
    {
        List<OrderDetailResponseDto> GetAll();

        OrderDetailResponseDto? GetById(int id);

        List<OrderDetailResponseDto> GetByOrderId(int orderId);

        int Create(OrderDetailCreateStandaloneDto detailDto);

        bool Update(int id, OrderDetailUpdateDto detailDto);

        bool Delete(int id);
    }
}