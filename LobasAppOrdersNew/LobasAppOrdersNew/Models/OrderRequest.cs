namespace LobasAppOrdersNew.Models
{
    public class OrderRequest
    {
        public int CustomerId { get; set; }

        public string Status { get; set; } = "Pendiente";

        public List<OrderDetailRequest> Details { get; set; } = new();
    }

    public class OrderDetailRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class OrderCartItemModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal => UnitPrice * Quantity;

        public string QuantityText => $"Qty: {Quantity}";

        public string UnitPriceText => $"${UnitPrice:N2}";

        public string SubtotalText => $"${Subtotal:N2}";
    }
}