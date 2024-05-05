using Domain.IService;
using Domain.Service;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.OrderDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace HotelApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/OrderDetail")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderService _orderService;

        public OrderDetailController(IOrderDetailService orderDetailService, IOrderService orderService)
        {
            _orderDetailService = orderDetailService;
            _orderService = orderService;
        }

        [HttpPost]
        [Route("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingListWithPropAndFilter(PagingListPropModel pagingListPropModel)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var orderDetails = await _orderDetailService.GetListByPageWithOrderPropAndFilterSearchText(pagingListPropModel);
            var total = _orderDetailService.GetTotal(pagingListPropModel);

            response.Content = new { data = orderDetails, total };

            return Ok(response);
        }

        [HttpGet]
        [Route("GetByOrderId")]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var result = await _orderDetailService.GetOrderDetailsByOrderId(orderId);

            response.Content = result;

            return Ok(response);
        }

        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var order = await _orderDetailService.GetById(id);

            response.Content = order;

            return Ok(response);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] OrderDetail order)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };
            var result = await _orderDetailService.Add(order);

            response.Content = result;

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("AddListExtraService")]
        public async Task<IActionResult> AddListExtraService([FromBody] IList<BookExtraService> bookExtraServices)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var userRequestId = User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value;

            var userId = userRequestId.IsNullOrEmpty() ? Guid.Empty : Guid.Parse(userRequestId);

            if (userId == Guid.Empty)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            var result = await _orderDetailService.AddListExtraService(bookExtraServices.ToList(), userId);

            if(result != ResponseEnum.Success.ToString())
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            var orderId = bookExtraServices.Select(s=>s.OrderId).Distinct().SingleOrDefault();
            var updateTotalResult = await _orderService.UpdateTotalPrice(orderId, bookExtraServices);

            if (updateTotalResult != ResponseEnum.Success.ToString())
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = bookExtraServices;

            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(OrderDetail orderUpdate)
        {
            var order = await _orderDetailService.Update(orderUpdate);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        #region

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(bool orderDesByCreate = false)
        {
            var orders = await _orderDetailService.GetAll(orderDesByCreate);
            return Ok(orders);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(Guid roomTypeId)
        {
            var result = await _orderDetailService.Remove(roomTypeId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        #endregion
    }
}
