using Domain.IService;
using Domain.Service;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;


namespace HotelApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/Room")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingListWithProp(PagingListPropModel pagingListPropModel)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var rooms = await _roomService.GetListByPageWithOrderPropAndFilterSearchText(pagingListPropModel);
            var total = _roomService.GetTotal(pagingListPropModel);

            response.Content = new { data = rooms, total };

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
            var rooms = await _roomService.GetById(id);

            response.Content = rooms;
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetByDepartmentId")]
        public async Task<IActionResult> GetRoomByDepartmentId(Guid departmentId)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var result = await _roomService.GetRoomByDepartmentId(departmentId);

            response.Content = result;

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetByRoomTypeId")]
        public async Task<IActionResult> GetRoomByRoomTypeId(Guid roomTypeId)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var result = await _roomService.GetRoomByRoomTypeId(roomTypeId);

            response.Content = result;

            return Ok(response);
        }


        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] Room room)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var result = await _roomService.Add(room);

            if (result == Guid.Empty)
            {
                response.ErrorMessage = ResponseEnum.NameIsExisted.ToString();
                response.IsSuccess = false;
                return BadRequest(response);
            }

            response.Content = room;

            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(Room roomUpdate)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };
            var result = await _roomService.Update(roomUpdate);
            if (!result.IsNullOrEmpty() && result != ResponseEnum.Success.ToString())
            {
                response.IsSuccess = false;
                response.ErrorMessage = result;
                return BadRequest(response);
            }

            response.Content = roomUpdate;
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

            var result = await _roomService.Remove(roomTypeId);
            if (result == ResponseEnum.SomewhereWrong.ToString())
            {
                response.IsSuccess = false;
                response.ErrorMessage = result;
                return BadRequest(response);
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

            var rooms = await _roomService.GetAll(orderDesByCreate);

            response.Content = rooms;

            return Ok(response);
        }

        [HttpGet]
        [Route("CreateData")]
        public async Task<IActionResult> CreateData()
        {
            IList<Room> rooms = new List<Room>();
            for (int i = 1; i < 17; i++)
            {
                var room = new Room
                {
                    IsActive = i % 2 == 0,
                    Status = "Available",
                    UpdateDate = DateTime.UtcNow,
                };
                await _roomService.Add(room);
            }
            return Ok();
        }

        #endregion
    }
}
