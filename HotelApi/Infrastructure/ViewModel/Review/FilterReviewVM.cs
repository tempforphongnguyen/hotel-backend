using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Review
{
    public class FilterReviewVM
    {
        public PagingListPropModel PagingListPropModel { get; set; }
        public int Star { get; set; }
        public Guid OrderId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid UserId { get; set; }
    }
}
