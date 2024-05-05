using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel
{
    public class PagingListPropModel
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool SortDesc { get; set; }
        public string SortBy { get; set; }
        public string SearchText { get; set; }
    }
}
