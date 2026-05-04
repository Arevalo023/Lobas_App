using LobasOrdersApi.DTOs;
using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using LobasOrdersApi.Services.Interfaces;

namespace LobasOrdersApi.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderDetailService(
            IOrderDetailRepository orderDetailRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public List<OrderDetailResponseDto> GetAll()
        {
            List<OrderDetail> details = _orderDetailRepository.GetAll();

            return details.Select(MapToResponseDto).ToList();
        }

        public OrderDetailResponseDto? GetById(int id)
        {
            OrderDetail? detail = _orderDetailRepository.GetById(id);

            if (detail == null)
            {
                return null;
            }

            return MapToResponseDto(detail);
        }

        public List<OrderDetailResponseDto> GetByOrderId(int orderId)
        {
            List<OrderDetail> details = _orderDetailRepository.GetByOrderId(orderId);

            return details.Select(MapToResponseDto).ToList();
        }

        public int Create(OrderDetailCreateStandaloneDto detailDto)
        {
            Order? order = _orderRepository.GetById(detailDto.OrderId);

            if (order == null)
            {
                throw new Exception("El pedido no existe");
            }

            Product? product = _productRepository.GetById(detailDto.ProductId);

            if (product == null)
            {
                throw new Exception("El producto no existe");
            }

            if (detailDto.Quantity <= 0)
            {
                throw new Exception("La cantidad debe ser mayor a cero");
            }

            if (product.Stock < detailDto.Quantity)
            {
                throw new Exception($"No hay stock suficiente para el producto: {product.Name}");
            }

            decimal subtotal = product.Price * detailDto.Quantity;

            OrderDetail detail = new OrderDetail
            {
                OrderId = detailDto.OrderId,
                ProductId = detailDto.ProductId,
                Quantity = detailDto.Quantity,
                UnitPrice = product.Price,
                Subtotal = subtotal
            };

            int newId = _orderDetailRepository.Create(detail);

            int newStock = product.Stock - detailDto.Quantity;
            _productRepository.UpdateStock(product.Id, newStock);

            RecalculateOrderTotal(detailDto.OrderId);

            return newId;
        }

        public bool Update(int id, OrderDetailUpdateDto detailDto)
        {
            OrderDetail? existingDetail = _orderDetailRepository.GetById(id);

            if (existingDetail == null)
            {
                return false;
            }

            Product? oldProduct = _productRepository.GetById(existingDetail.ProductId);

            if (oldProduct == null)
            {
                throw new Exception("El producto anterior no existe");
            }

            Product? newProduct = _productRepository.GetById(detailDto.ProductId);

            if (newProduct == null)
            {
                throw new Exception("El producto nuevo no existe");
            }

            if (detailDto.Quantity <= 0)
            {
                throw new Exception("La cantidad debe ser mayor a cero");
            }

            if (existingDetail.ProductId == detailDto.ProductId)
            {
                int availableStock = newProduct.Stock + existingDetail.Quantity;

                if (availableStock < detailDto.Quantity)
                {
                    throw new Exception($"No hay stock suficiente para el producto: {newProduct.Name}");
                }

                int updatedStock = availableStock - detailDto.Quantity;
                _productRepository.UpdateStock(newProduct.Id, updatedStock);
            }
            else
            {
                int restoredOldStock = oldProduct.Stock + existingDetail.Quantity;
                _productRepository.UpdateStock(oldProduct.Id, restoredOldStock);

                if (newProduct.Stock < detailDto.Quantity)
                {
                    throw new Exception($"No hay stock suficiente para el producto: {newProduct.Name}");
                }

                int newStock = newProduct.Stock - detailDto.Quantity;
                _productRepository.UpdateStock(newProduct.Id, newStock);
            }

            decimal subtotal = newProduct.Price * detailDto.Quantity;

            OrderDetail updatedDetail = new OrderDetail
            {
                OrderId = existingDetail.OrderId,
                ProductId = detailDto.ProductId,
                Quantity = detailDto.Quantity,
                UnitPrice = newProduct.Price,
                Subtotal = subtotal
            };

            bool updated = _orderDetailRepository.Update(id, updatedDetail);

            RecalculateOrderTotal(existingDetail.OrderId);

            return updated;
        }

        public bool Delete(int id)
        {
            OrderDetail? detail = _orderDetailRepository.GetById(id);

            if (detail == null)
            {
                return false;
            }

            Product? product = _productRepository.GetById(detail.ProductId);

            if (product != null)
            {
                int restoredStock = product.Stock + detail.Quantity;
                _productRepository.UpdateStock(product.Id, restoredStock);
            }

            bool deleted = _orderDetailRepository.Delete(id);

            RecalculateOrderTotal(detail.OrderId);

            return deleted;
        }

        private void RecalculateOrderTotal(int orderId)
        {
            List<OrderDetail> details = _orderDetailRepository.GetByOrderId(orderId);

            decimal total = details.Sum(d => d.Subtotal);

            _orderRepository.UpdateTotal(orderId, total);
        }

        private OrderDetailResponseDto MapToResponseDto(OrderDetail detail)
        {
            Product? product = _productRepository.GetById(detail.ProductId);

            return new OrderDetailResponseDto
            {
                Id = detail.Id,
                OrderId = detail.OrderId,
                ProductId = detail.ProductId,
                ProductName = product?.Name ?? "Producto no encontrado",
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                Subtotal = detail.Subtotal
            };
        }
    }
}