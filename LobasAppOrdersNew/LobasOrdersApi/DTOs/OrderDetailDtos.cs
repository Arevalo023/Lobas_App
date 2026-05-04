namespace LobasOrdersApi.DTOs
{
    public class OrderDetailCreateDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class OrderDetailCreateStandaloneDto
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class OrderDetailUpdateDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class OrderDetailResponseDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }
    }
}