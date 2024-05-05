using Domain.IService;
using Domain.Service;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace HotelApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/Order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingListWithPropAndFilter(PagingListPropModel pagingListPropModel)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var orders = await _orderService.GetListByPageWithOrderPropAndFilterSearchText(pagingListPropModel);
            var total = _orderService.GetTotal(pagingListPropModel);

            response.Content = new { data = orders, total };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("GetPagingWithStatusAndEmail")]
        public async Task<IActionResult> GetPagingWithStatusAndEmail(FilterOrderVM filterOrder)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            var orders = await _orderService.GetListByPageWithOrderPropAndFilterSearchText(filterOrder);
            var total = _orderService.GetTotal(filterOrder);

            response.Content = new { data = orders, total };
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var order = await _orderService.GetById(id);

            if(order is null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = order;
            return Ok(response);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] Order order)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var result =  await _orderService.Add(order);

            if(result == Guid.Empty)
            {
                response.IsSuccess= false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = order;

            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(Order orderUpdate)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var order = await _orderService.Update(orderUpdate);
            if (order == null)
            {
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                response.IsSuccess= false;
                return BadRequest(response);
            }

            response.Content = order;
            return Ok(response);
        }

        [HttpPut("UpdateStatusOrderAndPayment")]
        public async Task<IActionResult> UpdateStatusOrderAndPayment([FromBody] UpdateOrderVM updateOrder)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };
            var result = await _orderService.UpdateStatusOrderAndPayment(updateOrder);
            
            if(result != ResponseEnum.Success.ToString())
            {
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                response.IsSuccess = false;
                return BadRequest(response);
            }
            response.Content = updateOrder;

            return Ok(response);
        }

        [HttpPost]
        [Route("CreateOrderWithBooking")]
        public async Task<IActionResult> CreateOrderWithBooking(CreateOrderVM createOrder)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var result = await _orderService.AddNewOrderForBooking(createOrder);

            if(result == Guid.Empty)
            {
                response.ErrorMessage = ResponseEnum.RoomNotAvailable.ToString();
                response.IsSuccess= false;
                return BadRequest(response);
            }

            response.Content = new {Id = result, Status = StatusOrderEnum.WaitingPayment.ToString()};

            return Ok(response);
        }

        [HttpPut("RecalculateOrderStatus")]
        public async Task<IActionResult> RecalculateOrderStatus()
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            await _orderService.UpdateOrderExpCheckin();

            response.Content = ResponseEnum.Success.ToString();

            return Ok(response);
        }

        [HttpPost("GetReportOrder")]
        public async Task<IActionResult> GetReportOrder([FromBody]ReportOrderRequest reportOrder)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            response.Content = await _orderService.GetTotalOrderByDateTime(reportOrder);

            return Ok(response);
        }

        #region Do not use regularly
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(Guid roomTypeId)
        {
            var result = await _orderService.Remove(roomTypeId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(bool orderDesByCreate = false)
        {
            var orders = await _orderService.GetAll(orderDesByCreate);
            return Ok(orders);
        }
        #endregion
    }
}
