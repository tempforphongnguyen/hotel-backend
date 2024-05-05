using Infrastructure.Entities;
using Infrastructure.ViewModel.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IReviewService
    {
        Task<IList<Review>> GetListByPageWithOrderPropAndFilterSearchText(FilterReviewVM filterReview);
        int GetTotal(FilterReviewVM filterReview);
        Task<Guid> Add(Review review);
    }
}
