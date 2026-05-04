using LobasOrdersApi.Models;

namespace LobasOrdersApi.Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        List<OrderDetail> GetAll();

        OrderDetail? GetById(int id);

        List<OrderDetail> GetByOrderId(int orderId);

        int Create(OrderDetail orderDetail);

        bool Update(int id, OrderDetail orderDetail);

        bool Delete(int id);

        bool DeleteByOrderId(int orderId);
    }
}