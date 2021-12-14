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
    [Route("api/cms/vucvoucher")]
    [ApiController]
    public class VucVoucherController : ControllerBase
    {
        private readonly IVucVoucherHandler _interfaceHandler;
        public VucVoucherController(IVucVoucherHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        #region GET
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListVoucherByFilter(string query)
        {
            VucVoucherQueryModel model = new VucVoucherQueryModel();

            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    model = JsonConvert.DeserializeObject<VucVoucherQueryModel>(query);

                    result = await _interfaceHandler.GetListVoucherByFilter(model);
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
        [Route("all")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListVoucherAll(string query)
        {
            var result = await _interfaceHandler.GetListVoucherAll(query);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherByID(decimal id)
        {
            var result = await _interfaceHandler.GetVoucherByID(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("condition")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(decimal channelId, decimal issueBatchId, decimal voucherTypeId)
        {
            var result = await _interfaceHandler.GetListVoucherForMapping(channelId, issueBatchId, voucherTypeId);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusVoucher(VucVoucherCreateUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                CreateBy = requestInfo.UserName
            };

            var result = await _interfaceHandler.CreateAsync(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusVoucher(decimal id, VucVoucherCreateUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                LastModifyBy = requestInfo.UserName
            };

            var result = await _interfaceHandler.UpdateAsync(id,model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("{id}/approve")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveVoucher(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                CreateBy = requestInfo.UserName
            };

            var result = await _interfaceHandler.ApproveVoucher(id, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("{id}/reject")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectVoucher(decimal id)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                CreateBy = requestInfo.UserName
            };

            var result = await _interfaceHandler.RejectVoucher(id, baseModel);
            return RequestHelpers.TransformData(result);
        }

        //[HttpPut]
        //[Route("{id}/cancel")]
        //[ProducesResponseType(typeof(ResponseObject<VucVoucherBaseModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> CancelVoucher(decimal id)
        //{
        //    var requestInfo = RequestHelpers.GetRequestInfo(Request);
        //    var baseModel = new EVoucherBaseModel
        //    {
        //        CreateBy = requestInfo.UserName
        //    };

        //    var result = await _interfaceHandler.CancelVoucher(id, baseModel);
        //    return RequestHelpers.TransformData(result);
        //}
        #endregion
    }
}