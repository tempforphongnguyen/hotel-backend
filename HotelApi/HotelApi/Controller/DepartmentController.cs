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
    [Route("api/Department")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
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

            var departments = await _departmentService.GetListByPageWithOrderPropAndFilterSearchText(pagingListPropModel);

            var total= _departmentService.GetTotal(pagingListPropModel);

            response.Content = new { data = departments, total };

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

            var departments = await _departmentService.GetById(id);

            response.Content = departments;

            return Ok(response);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] Department department)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };
            await _departmentService.Add(department);
            
            response.Content = department;

            return Ok(department);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(Department departmentUpdate)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };
            var department = await _departmentService.Update(departmentUpdate);
            if (department == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            response.Content = department;

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
            var result = await _departmentService.Remove(roomTypeId);
            if (result.IsNullOrEmpty())
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return Ok(response);
            }

            response.Content = result;

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAll")]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAll(bool orderDesByCreate = false)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var departments = await _departmentService.GetAll(orderDesByCreate);

            response.Content = departments;

            return Ok(response);
        }

        #endregion
    }
}
