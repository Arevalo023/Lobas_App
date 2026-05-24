namespace LobasAppOrdersNew.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal Total { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public List<OrderDetailModel> Details { get; set; } = new();

        public string OrderNumberText => $"Order #{Id}";

        public string TotalText => $"${Total:N2}";

        public string DateText => OrderDate.ToString("dd/MM/yyyy");

        public string StatusText => string.IsNullOrWhiteSpace(Status)
            ? "Sin estatus"
            : Status;
    }

    public class OrderDetailModel
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }

        public string SubtotalText => $"${Subtotal:N2}";
    }
}