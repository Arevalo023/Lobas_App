namespace LobasAppOrdersNew.Models
{
    public class AddressRequest
    {
        public int CustomerId { get; set; }

        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public bool IsMain { get; set; }
    }
}
