using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Room
{
    public class RoomBookingRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxChild { get; set; }
        public int MaxPerson { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid RoomTypeId { get; set; }
    }
}
