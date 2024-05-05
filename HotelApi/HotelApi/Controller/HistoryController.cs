using Domain.IService;
using Domain.Service;
using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace HotelApi.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/History")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
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

            var histories = await _historyService.GetListByPageWithOrderPropAndFilterSearchText(pagingListPropModel);

            var total = _historyService.GetTotal(pagingListPropModel);

            response.Content = new { data = histories, total };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("GetByOrderId")]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var response = new MainResponse()
            {
                IsSuccess = true
            };

            var histories = await _historyService.GetByOrderId(orderId);

            response.Content = histories;

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("GetById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var histories = await _historyService.GetById(id);
            return Ok(histories);
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add([FromBody] History history)
        {
            await _historyService.Add(history);
            return Ok();
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(History historyUpdate)
        {
            var history = await _historyService.Update(historyUpdate);
            if (history == null)
            {
                return NotFound();
            }
            return Ok(history);
        }
       
        #region Do not use regularly
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(Guid roomTypeId)
        {
            var result = await _historyService.Remove(roomTypeId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("CreateData")]
        public async Task<IActionResult> CreateData()
        {
            IList<History> historys = new List<History>();
            for (int i = 1; i < 17; i++)
            {
                var history = new History
                {
                    IsActive = i % 2 == 0,
                    Status = "Available",
                    UpdateDate = DateTime.UtcNow,
                };
                await _historyService.Add(history);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll(bool orderDesByCreate = false)
        {
            var historys = await _historyService.GetAll(orderDesByCreate);
            return Ok(historys);
        }
        #endregion
    }
}
