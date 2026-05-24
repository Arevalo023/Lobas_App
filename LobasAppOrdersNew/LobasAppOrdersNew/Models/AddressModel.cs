namespace LobasAppOrdersNew.Models
{
    public class AddressModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        public string FullAddressText => $"{Street}, {City}, {State}, {ZipCode}";

        public string MainAddressText => IsMain ? "Principal" : "Secundaria";
    }
}
