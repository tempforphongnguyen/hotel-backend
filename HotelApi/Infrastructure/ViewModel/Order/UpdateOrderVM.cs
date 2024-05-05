using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Order
{
    public class UpdateOrderVM
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public string PaymentNote { get; set; }
        public bool IsPaid { get; set; }
        public Guid StaffUserId { get; set; }
    }
}
