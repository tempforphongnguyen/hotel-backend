using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel
{
    public class ResponseModel
    {
        public string? Message { get; set; }
        public object? Data { get; set; }
        public bool? IsSuccess { get; set; }
    }
}
