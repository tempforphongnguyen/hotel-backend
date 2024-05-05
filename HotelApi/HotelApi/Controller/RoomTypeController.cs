using Domain.IService;
using Domain.Service;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Auth.User;
using Infrastructure.ViewModel.RoomType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace HotelApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/RoomType")]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly IOrderService _orderService;
        
        public RoomTypeController(IRoomTypeService roomTypeService, IOrderService orderService)
        {
            _roomTypeService = roomTypeService;
            _orderService = orderService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingWithProp(PagingListPropModel pagingListPropModel)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var roomTypes = await _roomTypeService.GetListByPageWithOrderPropAndFilterSearchText(pagingListPropModel);
            var total = _roomTypeService.GetTotal(pagingListPropModel);

            response.Content = new { data = roomTypes, total };

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

            var roomType = await _roomTypeService.GetById(id);

            response.Content = roomType;
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetByDepartmentId")]
        public async Task<IActionResult> GetRoomTypeByDepartmentId(Guid departmentId)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var roomTypes = await _roomTypeService.GetRoomTypeByDepartmentId(departmentId);

            response.Content = roomTypes;

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetRoomTypeAvailableInDepartment")]
        public async Task<IActionResult> GetRoomTypeAvailableInDepartment([FromBody] FilterRoom roomTypeAvailableInDepartment)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            await _orderService.UpdateOrderWaitingPayment();

            var roomType = await _roomTypeService.GetRoomTypesAvailableInDepartment(roomTypeAvailableInDepartment);

            response.Content = new { data = roomType, total = roomType.Count };

            return Ok(response);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] RoomType roomType)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            var result = await _roomTypeService.Add(roomType);
            if (result == Guid.Empty)
            {
                response.ErrorMessage = ResponseEnum.NameIsExisted.ToString();
                response.IsSuccess = false;
                return BadRequest(response);
            }

            response.Content = roomType;

            return Ok(response);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] RoomType roomType)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            var result = await _roomTypeService.Update(roomType);

            if (!result.IsNullOrEmpty() && result != ResponseEnum.Success.ToString())
            {
                response.ErrorMessage = result;
                response.IsSuccess = false;
                return BadRequest(response);
            }

            response.Content = roomType;

            return Ok(response);
        }

        #region Do not use regularly

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(Guid roomTypeId)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };
            var result = await _roomTypeService.Remove(roomTypeId);

            if (result.IsNullOrEmpty())
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Not found Department";
                return Ok(response);
            }

            response.Content = result;
            return Ok(response);
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(bool orderDesByCreate = false)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };
            var roomTypes = await _roomTypeService.GetAll(orderDesByCreate);

            response.Content = roomTypes;

            return Ok(response);
        }
        
        #endregion
    }
}
