namespace LobasAppOrdersNew.Models
{
    public class CustomerRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}