using Infrastructure.ViewModel.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Order
{
    public class CreateOrderVM
    {
        public BookingRoom BookingRoom { get; set; }
        public List<BookExtraService> BookExtraService { get; set; }
        public bool Paid { get; set; }
        public string Note { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public double Discount { get; set; }
        public string PaymentNote { get; set; }
    }

    public class BookingRoom
    {
        public Guid DepartmentId { get; set; }
        public int MaxPerson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid RoomTypeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitRoomTypePrice { get; set; }
        public string RoomTypeName{ get; set; }
    }
}
