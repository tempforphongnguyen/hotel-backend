using Domain.IService;
using Domain.Service;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.Vnpay;
using Microsoft.AspNetCore.Mvc;

namespace HotelApi.Controller
{
    [ApiController]
    [Route("api/Vnpay")]
    public class VnpayController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;

        public VnpayController(IVnpayService vnpayService)
        {
            _vnpayService = vnpayService;
        }

        [HttpPost]
        [Route("GetUrlPaymentVnpay")]
        public IActionResult GetUrlPaymentVnpay(PaymentInformationModel model)
        {
            var response = new MainResponse()
            {
                IsSuccess = true,
            };

            var url = _vnpayService.CreatePaymentUrl(model, HttpContext);

            response.Content = url;
            return Ok(response);
        }
    }
}
