using Domain.IService;
using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controller
{

    [Authorize]
    [ApiController]
    [Route("api/Review")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService )
        {
            _reviewService = reviewService;
        }

        [AllowAnonymous]
        [HttpPost("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingWithPropAsync(FilterReviewVM filterReview)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            var result = await _reviewService.GetListByPageWithOrderPropAndFilterSearchText(filterReview);
            var total = _reviewService.GetTotal(filterReview);

            response.Content = new { data = result, total };

            return Ok(response);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(Review review)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            await _reviewService.Add(review);

            response.Content = review;

            return Ok(response);
        }
    }
}
