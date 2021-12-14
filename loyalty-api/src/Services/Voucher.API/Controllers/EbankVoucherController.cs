using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voucher.API.Interface;
using Voucher.API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;
using System;

namespace Voucher.API.Controllers
{
    [Route("api/ebankvoucher")]
    [ApiController]
    public class EbankVoucherController : ControllerBase
    {
        private readonly IEbankVoucherHandler _interfaceHandler;
        public EbankVoucherController(IEbankVoucherHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<EbankVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherList(string customerId, string channelId, string tranType, decimal tranAmount)
        {
            var result = await _interfaceHandler.GetVoucherList(customerId, channelId, tranType, tranAmount);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("update")]
        [ProducesResponseType(typeof(ResponseObject<EbankVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusVoucher(string update)
        {

            EbankVoucherUpdateModel updateModel = new EbankVoucherUpdateModel();
            Response result = null;

            if (!String.IsNullOrEmpty(update))
            {
                try
                {
                    updateModel = JsonConvert.DeserializeObject<EbankVoucherUpdateModel>(update);

                    result = await _interfaceHandler.UpdateElgVoucherSync(updateModel);
                }
                catch (Exception ex)
                {
                    result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
                }
            }
            else
            {
                result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
            }


            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("checkvoucher")]
        [ProducesResponseType(typeof(ResponseObject<EbankVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckStatusVoucher(string check)
        {

            EbankVoucherCheckModel checkvoucherModel = new EbankVoucherCheckModel();
            Response result = null;

            if (!String.IsNullOrEmpty(check))
            {
                try
                {
                    checkvoucherModel = JsonConvert.DeserializeObject<EbankVoucherCheckModel>(check);

                    result = await _interfaceHandler.CheckElgVoucherSync(checkvoucherModel);
                }
                catch (Exception ex)
                {
                    result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
                }
            }
            else
            {
                result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
            }


            return RequestHelpers.TransformData(result);
        }
    }
}