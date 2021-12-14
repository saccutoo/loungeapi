using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;
using System;

namespace API.Controllers
{
    [Route("api/promo/promotion")]
    [ApiController]
    public class PrmPromotionController : ControllerBase
    {
        private readonly IPrmPromotionHandler _iPrmPromotionHandler;
        public PrmPromotionController(IPrmPromotionHandler iPrmPromotionHandler)
        {
            _iPrmPromotionHandler = iPrmPromotionHandler;
        }

        #region GET
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            PrmPromotionQueryModel queryModel = new PrmPromotionQueryModel();
            Response result = null;

            if (!string.IsNullOrEmpty(query))
            {              
                queryModel = JsonConvert.DeserializeObject<PrmPromotionQueryModel>(query);
                var requestInfo = RequestHelpers.GetRequestInfo(Request);
                if (requestInfo != null) queryModel.DeptId = requestInfo.DeptId;
                result = await _iPrmPromotionHandler.GetByFilterAsync(queryModel);
            }
            else result = new ResponseError(Utils.StatusCode.Fail, "Thiếu dữ liệu truy vấn");

            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _iPrmPromotionHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-valid-by-trans-log")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListPromoValidByTransLog(decimal transLogId)
        {
            string employeePosCd = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) employeePosCd = requestInfo.PosCd;
            var result = await _iPrmPromotionHandler.GetListPromoValidByTransLog(transLogId);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-valid-by-legacy-ref")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListPromoValidByLegacyRefNo(string legacyRefNo, string license, string phone)
        {
            string employeePosCd = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) employeePosCd = requestInfo.PosCd;
            var result = await _iPrmPromotionHandler.GetListPromoValidByLegacyRefNo(legacyRefNo, license, phone, employeePosCd);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-valid-gift-by-legacy-ref")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListGiftValidByLegacyRefNo(string legacyRefNo, string license, string phone)
        {
            var result = await _iPrmPromotionHandler.GetListGiftValidByLegacyRefNo(legacyRefNo, license, phone);
            return RequestHelpers.TransformData(result);
        }

        #endregion

        #region CRUD
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] PrmPromotionCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null)
            {
                model.CreateBy = requestInfo.UserName;
                model.CreateByPos = requestInfo.PosCd;
                model.CreateByDept = requestInfo.DeptId;
            }
            var result = await _iPrmPromotionHandler.CreateAsync(model);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] PrmPromotionUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.LastModifiedBy = requestInfo.UserName;
            var result = await _iPrmPromotionHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByIdAsync(decimal id)
        {

            var result = await _iPrmPromotionHandler.DeleteByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region APPROVE/REJECT
        [HttpPut, Route("{id}/approve")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveByIdAsync(decimal id)
        {
            string approvedBy = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) approvedBy = requestInfo.UserName;
            var result = await _iPrmPromotionHandler.ApproveByIdAsync(id, approvedBy);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}/reject")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectByIdAsync(decimal id, string approvedComment)
        {
            string approvedBy = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) approvedBy = requestInfo.UserName;
            var result = await _iPrmPromotionHandler.RejectByIdAsync(id, approvedBy, approvedComment);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}/{status}")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusAsync(decimal id, string status)
        {

            var result = await _iPrmPromotionHandler.UpdateStatusAsync(id, status);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}