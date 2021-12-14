using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voucher.API.Interface;
using Voucher.API.Models;
using System.Threading.Tasks;
using Utils;
using API.Filters;
using Newtonsoft.Json;
using System;

namespace Voucher.API.Controllers
{
    [Route("api/cms/vucmapvouchercust")]
    [ApiController]
    public class VucMapVoucherCustController : ControllerBase
    {
        private readonly IVucMapVoucherCust _interfaceHandler;
        public VucMapVoucherCustController(IVucMapVoucherCust interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        #region GET
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<VucMapVoucherCustBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMapVoucherListByFilter(string query)
        {
            VucMapVoucherCustQueryModel queryModel = new VucMapVoucherCustQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<VucMapVoucherCustQueryModel>(query);

                    result = await _interfaceHandler.GetMapVoucherListByFilter(queryModel);
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
        #endregion

        [HttpPost]
        [Route("validate")]
        [ProducesResponseType(typeof(ResponseObject<VucMapVoucherCustBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateVoucher(decimal voucherId, string transType, decimal numOfVoucherTarget)
        {
            var result = await _interfaceHandler.ValidateVoucherCustomer(voucherId, transType, numOfVoucherTarget);
            return RequestHelpers.TransformData(result);
        }

        [HttpPost]
        [Route("map/customer-ebank")]
        [ProducesResponseType(typeof(ResponseObject<VucMapVoucherCustBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MapCustVoucherEbank(VucMapVoucherCustMappingModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                LastModifyBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.MapVoucherCustomerEbank(model, baseModel);
            return RequestHelpers.TransformData(result);
        }
        [HttpPost]
        [Route("map/customer")]
        [ProducesResponseType(typeof(ResponseObject<VucMapVoucherCustBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MapCustVoucher(VucMapVoucherCustMappingModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                LastModifyBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.MapVoucherCustomerEbank(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("cancel")]
        [ProducesResponseType(typeof(ResponseObject<VucMapVoucherCustBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelVoucherMap (decimal mapId)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                LastModifyBy = requestInfo.UserName
            };

            var result = await _interfaceHandler.CancelVoucherMapping(mapId, baseModel);
            return RequestHelpers.TransformData(result);
        }
    }
}