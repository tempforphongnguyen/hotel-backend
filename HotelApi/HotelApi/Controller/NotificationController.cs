using Domain.IService;
using Domain.Service;
using Infrastructure.Entities;
using Infrastructure.ViewModel.Review;
using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace HotelApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/Notification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingWithPropAsync(FilterNotification filterNotification)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            var result = await _notificationService.GetListByPageWithOrderPropAndFilterSearchText(filterNotification);

            response.Content = new { data = result };

            return Ok(response);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(Notification notification)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            await _notificationService.Add(notification);

            response.Content = notification;

            return Ok(response);
        }


        [HttpGet("CheckAnyNoti")]
        public async Task<IActionResult> CheckAnyNoti(Guid userId)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            var result = await _notificationService.CheckNotiUnRead(userId);

            response.Content = result;

            return Ok(response);
        }
    }
}
