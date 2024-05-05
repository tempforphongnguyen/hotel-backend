using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.RoomType
{
    public class FilterRoom
    {
        public Guid DepartmentId { get; set; }
        public int MaxPerson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid RoomTypeId { get; set; }
    }
}
