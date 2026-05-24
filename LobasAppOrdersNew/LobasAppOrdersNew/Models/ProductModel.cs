namespace LobasAppOrdersNew.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PriceText => $"${Price:N2}";

        public string StockText => $"Stock: {Stock}";
    }
}