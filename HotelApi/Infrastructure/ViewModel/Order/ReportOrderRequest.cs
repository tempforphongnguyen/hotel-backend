using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Order
{
    public class ReportOrderRequest
    {
        public List<DateUnit> DateUnits { get; set; }
        public Guid DepartmentId { get; set; }
    }
    public class DateUnit
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
