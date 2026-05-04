namespace LobasOrdersApi.DTOs
{
    public class AddressCreateDto
    {
        public int CustomerId { get; set; }

        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public bool IsMain { get; set; }
    }

    public class AddressUpdateDto
    {
        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public bool IsMain { get; set; }
    }

    public class AddressResponseDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string ZipCode { get; set; } = string.Empty;

        public bool IsMain { get; set; }
    }
}