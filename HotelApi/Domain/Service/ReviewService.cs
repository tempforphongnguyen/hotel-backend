using Domain.IService;
using Domain.Static;
using Infrastructure.Entities;
using Infrastructure.ViewModel.Order;
using Infrastructure.ViewModel.Review;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IRepository<Review> _repository;
        private readonly IUnitOfWork _unitOfWork;
        public ReviewService(IRepository<Review> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Review>> GetListByPageWithOrderPropAndFilterSearchText(FilterReviewVM filterReview)
        {
            Expression<Func<Review, bool>> mainCondition = null;
            
            Expression<Func<Review, bool>> starCondition = null;
            if (filterReview.Star > 0)
            {
                starCondition = s => s.Star == filterReview.Star;
                mainCondition = starCondition;
            }

            Expression<Func<Review, bool>> userCondition = null;
            if (filterReview.UserId != Guid.Empty)
            {
                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, userCondition);
                }
                else
                {
                    mainCondition = userCondition;
                }
            }

            Expression<Func<Review, bool>> orderIdCondition = null;
            if (filterReview.OrderId != Guid.Empty)
            {
                orderIdCondition = s => filterReview.OrderId == s.OrderId;

                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, orderIdCondition);
                }
                else
                {
                    mainCondition = orderIdCondition;
                }
            }

            Expression<Func<Review, bool>> roomTypeCondition = null;
            if (filterReview.RoomTypeId != Guid.Empty)
            {
                roomTypeCondition = s => s.RoomTypeId == filterReview.RoomTypeId;

                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, roomTypeCondition);
                }
                else
                {
                    mainCondition = roomTypeCondition;
                }
            }

            var reviews = mainCondition == null ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(
                                                                        filterReview.PagingListPropModel,
                                                                        include: s=>s.Include(u=>u.User))
                                                                    .ConfigureAwait(true)
                                       : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(
                                                                pagingListPropModel: filterReview.PagingListPropModel,
                                                                predicate: mainCondition,
                                                                include: s => s.Include(u => u.User))
                                                            .ConfigureAwait(true);

            return reviews.ToList();
        }

        public int GetTotal(FilterReviewVM filterReview)
        {
            Expression<Func<Review, bool>> mainCondition = null;

            Expression<Func<Review, bool>> starCondition = null;
            if (filterReview.Star > 0)
            {
                starCondition = s => s.Star == filterReview.Star;
                mainCondition = starCondition;
            }

            Expression<Func<Review, bool>> userCondition = null;
            if (filterReview.UserId != Guid.Empty)
            {
                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, userCondition);
                }
                else
                {
                    mainCondition = userCondition;
                }
            }

            Expression<Func<Review, bool>> orderIdCondition = null;
            if (filterReview.OrderId != Guid.Empty)
            {
                orderIdCondition = s => filterReview.OrderId == s.OrderId;

                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, orderIdCondition);
                }
                else
                {
                    mainCondition = orderIdCondition;
                }
            }

            Expression<Func<Review, bool>> roomTypeCondition = null;
            if (filterReview.RoomTypeId != Guid.Empty)
            {
                roomTypeCondition = s => s.RoomTypeId == filterReview.RoomTypeId;

                if (mainCondition != null)
                {
                    mainCondition = HelperStaticService.And(mainCondition, roomTypeCondition);
                }
                else
                {
                    mainCondition = roomTypeCondition;
                }
            }

            var total = mainCondition == null ?  _repository.GetTotalEntity() : _repository.GetTotalEntity(mainCondition);
            return total;
        }

        public async Task<Guid> Add(Review review)
        {
            _repository.Add(review);
            await _unitOfWork.CommitChangesAsync();

            return review.Id;
        }
    }
}
