using LobasOrdersApi.DTOs;
using LobasOrdersApi.Models;
using LobasOrdersApi.Repositories.Interfaces;
using LobasOrdersApi.Services.Interfaces;

namespace LobasOrdersApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public List<OrderResponseDto> GetAll()
        {
            List<Order> orders = _orderRepository.GetAll();

            return orders.Select(MapToResponseDto).ToList();
        }

        public OrderResponseDto? GetById(int id)
        {
            Order? order = _orderRepository.GetById(id);

            if (order == null)
            {
                return null;
            }

            return MapToResponseDto(order);
        }

        public List<OrderResponseDto> GetByCustomerId(int customerId)
        {
            List<Order> orders = _orderRepository.GetByCustomerId(customerId);

            return orders.Select(MapToResponseDto).ToList();
        }

        public List<OrderResponseDto> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAll();
            }

            List<Order> orders = _orderRepository.Search(searchTerm.Trim());

            return orders.Select(MapToResponseDto).ToList();
        }

        public int Create(OrderCreateDto orderDto)
        {
            Customer? customer = _customerRepository.GetById(orderDto.CustomerId);

            if (customer == null)
            {
                throw new Exception("El cliente no existe");
            }

            if (orderDto.Details == null || orderDto.Details.Count == 0)
            {
                throw new Exception("El pedido debe tener al menos un producto");
            }

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            decimal total = 0;

            foreach (OrderDetailCreateDto detailDto in orderDto.Details)
            {
                if (detailDto.Quantity <= 0)
                {
                    throw new Exception("La cantidad debe ser mayor a cero");
                }

                Product? product = _productRepository.GetById(detailDto.ProductId);

                if (product == null)
                {
                    throw new Exception($"El producto con Id {detailDto.ProductId} no existe");
                }

                if (product.Stock < detailDto.Quantity)
                {
                    throw new Exception($"No hay stock suficiente para el producto: {product.Name}");
                }

                decimal subtotal = product.Price * detailDto.Quantity;

                OrderDetail detail = new OrderDetail
                {
                    ProductId = product.Id,
                    Quantity = detailDto.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                };

                orderDetails.Add(detail);
                total += subtotal;
            }

            Order order = new Order
            {
                CustomerId = orderDto.CustomerId,
                Total = total,
                Status = string.IsNullOrWhiteSpace(orderDto.Status)
                    ? "Pendiente"
                    : orderDto.Status.Trim()
            };

            return _orderRepository.CreateWithDetailsAndStockUpdate(order, orderDetails);
        }

        public bool Update(int id, OrderUpdateDto orderDto)
        {
            Order? existingOrder = _orderRepository.GetById(id);

            if (existingOrder == null)
            {
                return false;
            }

            Customer? customer = _customerRepository.GetById(orderDto.CustomerId);

            if (customer == null)
            {
                throw new Exception("El cliente no existe");
            }

            if (string.IsNullOrWhiteSpace(orderDto.Status))
            {
                throw new Exception("El estatus del pedido es obligatorio");
            }

            Order order = new Order
            {
                CustomerId = orderDto.CustomerId,
                Status = orderDto.Status.Trim()
            };

            return _orderRepository.Update(id, order);
        }

        public bool UpdateStatus(int id, OrderStatusUpdateDto statusDto)
        {
            if (string.IsNullOrWhiteSpace(statusDto.Status))
            {
                throw new Exception("El estatus del pedido es obligatorio");
            }

            return _orderRepository.UpdateStatus(id, statusDto.Status.Trim());
        }

        public bool Delete(int id)
        {
            return _orderRepository.Delete(id);
        }

        private OrderResponseDto MapToResponseDto(Order order)
        {
            Customer? customer = _customerRepository.GetById(order.CustomerId);

            List<OrderDetail> details = _orderDetailRepository.GetByOrderId(order.Id);

            List<OrderDetailResponseDto> detailDtos = details.Select(detail =>
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
            }).ToList();

            return new OrderResponseDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = customer?.Name ?? "Cliente no encontrado",
                OrderDate = order.OrderDate,
                Total = order.Total,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                IsActive = order.IsActive,
                Details = detailDtos
            };
        }
    }
}
