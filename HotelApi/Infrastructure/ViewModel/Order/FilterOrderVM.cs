using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Order
{
    public class FilterOrderVM
    {
        public PagingListPropModel PagingListPropModel { get; set; }
        public List<string> Status { get; set; }
        public Guid UserId { get; set; }
        public string SearchEmailOrRoom { get; set; }
        public Guid DepartmentId { get; set; }
        public bool IsReviewed { get; set; }
        public bool IsReviewFilter { get; set; }
    }
}
