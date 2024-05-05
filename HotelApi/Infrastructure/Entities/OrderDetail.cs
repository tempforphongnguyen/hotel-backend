using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class OrderDetail : BaseEntity
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxPerson { get; set; }
        public bool Paid { get; set; }
        public bool IsRoom { get; set; }
        public Guid OriginalId { get; set; }
        public Guid OriginalRoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public string PaymentNote { get; set; }
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
