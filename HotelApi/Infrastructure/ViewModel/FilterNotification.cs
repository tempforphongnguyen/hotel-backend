using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel
{
    public class FilterNotification
    {
        public PagingListPropModel PagingListPropModel{ get; set; }
        public Guid UserId { get; set; }
    }
}
