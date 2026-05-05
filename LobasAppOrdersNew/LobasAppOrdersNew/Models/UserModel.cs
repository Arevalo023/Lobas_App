using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobasAppOrdersNew.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string AuthProvider { get; set; } = string.Empty;

        public string? ProviderUserId { get; set; }

        public bool BiometricEnabled { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}