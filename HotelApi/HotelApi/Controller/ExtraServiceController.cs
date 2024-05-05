using Domain.IService;
using Domain.Service;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace HotelApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/ExtraService")]
    public class ExtraServiceController : ControllerBase
    {
        private readonly IExtraServiceService _extraServiceService;

        public ExtraServiceController(IExtraServiceService extraServiceService)
        {
            _extraServiceService = extraServiceService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("GetPagingWithProp")]
        public async Task<IActionResult> GetPagingWithProp(PagingListPropModel pagingListPropModel)
        {
            var extraServices = await _extraServiceService.GetListByPageWithOrderPropAndFilterSearchText(pagingListPropModel);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };
            var total = _extraServiceService.GetTotal(pagingListPropModel);

            response.Content = new { data = extraServices, total };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = new MainResponse();

            var extraServices = await _extraServiceService.GetById(id);

            if(extraServices == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.IsSuccess = true;
            response.Content = extraServices;

            return Ok(response);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] ExtraService extraService)
        {
            var result = await _extraServiceService.Add(extraService);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (result == Guid.Empty)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = extraService;

            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(ExtraService extraServiceUpdate)
        {
            var extraService = await _extraServiceService.Update(extraServiceUpdate);

            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (extraService == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();

                return BadRequest(response);
            }

            response.Content = extraService;

            return Ok(response);
        }

        #region Do not use regularly

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(bool orderDesByCreate = false)
        {
            var extraServices = await _extraServiceService.GetAll(orderDesByCreate);
            return Ok(extraServices);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(Guid roomTypeId)
        {
            var result = await _extraServiceService.Remove(roomTypeId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        #endregion
    }
}
