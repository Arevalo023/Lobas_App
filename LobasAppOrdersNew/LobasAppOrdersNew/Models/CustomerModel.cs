namespace LobasAppOrdersNew.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string EmailText => string.IsNullOrWhiteSpace(Email)
            ? "No email"
            : Email;

        public string PhoneText => string.IsNullOrWhiteSpace(Phone)
            ? "No phone"
            : Phone;
    }
}