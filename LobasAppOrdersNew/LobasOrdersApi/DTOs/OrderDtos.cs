namespace LobasOrdersApi.DTOs
{
    public class OrderCreateDto
    {
        public int CustomerId { get; set; }

        public string Status { get; set; } = "Pendiente";

        public List<OrderDetailCreateDto> Details { get; set; } = new List<OrderDetailCreateDto>();
    }

    public class OrderUpdateDto
    {
        public int CustomerId { get; set; }

        public string Status { get; set; } = string.Empty;
    }

    public class OrderStatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public List<OrderDetailResponseDto> Details { get; set; } = new List<OrderDetailResponseDto>();
    }
}