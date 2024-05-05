using Infrastructure.ViewModel.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.OrderDetail
{
    public class OrderDetailFormatVM
    {
        public Guid OrderId { get; set; }
        public Entities.Order Order { get; set; }
        public RoomInformation RoomInformation { get; set; }
        public IList<ExtraServiceInformation> ExtraServiceInformations { get; set; }
        public UserInfo UserInfo { get; set; }
    }
    public class RoomInformation
    {
        public Guid Id { get; set; }
        public Guid OriginalId { get; set; }
        public Guid OriginalRoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxPerson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsRoom { get; set; }
        public bool Paid { get; set; }
        public string PaymentNote { get; set; }
    }

    public class ExtraServiceInformation
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool Paid { get; set; }
        public Guid OriginalId { get; set; }
        public string PaymentNote { get; set; }
    }
}
