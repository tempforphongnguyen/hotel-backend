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
    [ApiController]
    [Route("api/Upload")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly IUploadFileService _uploadFileService;
        
        public UploadController (IUploadFileService uploadFileService)
        {
            _uploadFileService = uploadFileService;
        }

        [HttpPost]
        [Route("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            if (file == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ResponseEnum.SomewhereWrong.ToString();
                return BadRequest(response);
            }

            var result = await _uploadFileService.UploadImage(file);

            response.Content = result;

            return Ok(response);
        }
        
    }
}
