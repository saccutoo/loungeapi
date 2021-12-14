using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voucher.API.Interface;
using Voucher.API.Models;
using System.Threading.Tasks;
using Utils;
using API.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Voucher.API.Controllers
{
    [Route("api/vucissuebath")]
    [ApiController]
    public class VucIssueBatchController : ControllerBase
    {
        private readonly IVucIssueBatchHandler _interfaceHandler;
        public VucIssueBatchController(IVucIssueBatchHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<VucIssueBatchBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIssueBathById(string id)
        {
            var result = await _interfaceHandler.GetIssueBathById(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<VucIssueBatchBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListIssueBath(string query)
        {
            var result = await _interfaceHandler.GetListIssueBath(query);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<VucIssueBatchBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListByFilter(string query)
        {
            VucVoucherQueryModel queryModel = new VucVoucherQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<VucVoucherQueryModel>(query);

                    result = await _interfaceHandler.GetListByFilter(queryModel);
                }
                catch (Exception ex)
                {
                    result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
                }
            } else
            {
                result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
            }
               
            
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        [HttpPost]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<VucIssueBatchBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] VucIssueBatchCreateUpdateModel model)
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
        //[Route("{id}")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<VucIssueBatchBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(string id, [FromBody] VucIssueBatchCreateUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                LastModifyBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateAsync(id, model, baseModel);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}