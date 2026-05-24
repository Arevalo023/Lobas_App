namespace LobasAppOrdersNew.Models
{
    public class OrderUpdateRequest
    {
        public int CustomerId { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}