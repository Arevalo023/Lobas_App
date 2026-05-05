using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobasAppOrdersNew.Models
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = string.Empty;

        public T? User { get; set; }
    }
}