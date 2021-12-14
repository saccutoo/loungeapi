using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/promo/transaction-log")]
    [ApiController]
    public class PrmTransactionLogController : ControllerBase
    {
        private readonly IPrmTransactionLogHandler _iPrmTransactionLogHandler;
        public PrmTransactionLogController(IPrmTransactionLogHandler iPrmTransactionLogHandler)
        {
            _iPrmTransactionLogHandler = iPrmTransactionLogHandler;
        }

        #region GET
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            PrmTransactionLogQueryModel queryModel = new PrmTransactionLogQueryModel();
            Response result = null;

            if (!string.IsNullOrEmpty(query))
            {
                queryModel = JsonConvert.DeserializeObject<PrmTransactionLogQueryModel>(query);
                var requestInfo = RequestHelpers.GetRequestInfo(Request);
                if (requestInfo != null) queryModel.Pos = string.IsNullOrEmpty(queryModel.Pos) ? requestInfo.PosCd : queryModel.Pos;
                result = await _iPrmTransactionLogHandler.GetByFilterAsync(queryModel);
            }
            else result = new ResponseError(Utils.StatusCode.Fail, "Thiếu dữ liệu truy vấn");

            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}/list-accounting")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListAccountingByTransactionId(decimal id)
        {
            var result = await _iPrmTransactionLogHandler.GetListAccountingByTransactionId(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _iPrmTransactionLogHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("check-acct-tide-is-closed-by-trans-log")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckAcctTideIsClosedByTransLog(decimal transLogId)
        {
            var posCD = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null)
            {
                posCD = requestInfo.PosCd;
            }
            var result = await _iPrmTransactionLogHandler.CheckAcctTideIsClosedByTransLog(transLogId, posCD);
            return RequestHelpers.TransformData(result);
        }

        //[HttpGet]
        //[Route("get-valid-by-deposit")]
        //[ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetListPromoValidByDepositNo(string depositNo, string license)
        //{
        //    var result = await _iPrmPromotionHandler.GetListPromoValidByDepositNo(depositNo, license);
        //    return RequestHelpers.TransformData(result);
        //}
        #endregion

        #region CRUD
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateLogAsync([FromBody] PrmTransactionLogCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null)
            {
                model.Pos = requestInfo.PosCd;
                model.DeptId = requestInfo.DeptId;
            }
            var result = await _iPrmTransactionLogHandler.CreateLogAsync(model);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] List<PrmProductTransactionLogCreateModel> listPrmProductTransactionLogModel)
        {

            var result = await _iPrmTransactionLogHandler.UpdateLogAsync(id, listPrmProductTransactionLogModel);
            return RequestHelpers.TransformData(result);
        }

        //[HttpDelete, Route("{id}")]
        //[ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> DeleteByIdAsync(decimal id)
        //{

        //    var result = await _iPrmPromotionHandler.DeleteByIdAsync(id);
        //    return RequestHelpers.TransformData(result);
        //}
        #endregion

        #region APPROVE/REJECT
        [HttpPut, Route("{id}/approve")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveByIdAsync(decimal id)
        {
            string approvedBy = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) approvedBy = requestInfo.UserName;
            var result = await _iPrmTransactionLogHandler.ApproveByIdAsync(id, approvedBy);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}/reject")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectByIdAsync(decimal id, string approvedComment)
        {
            string approvedBy = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) approvedBy = requestInfo.UserName;
            var result = await _iPrmTransactionLogHandler.RejectByIdAsync(id, approvedBy, approvedComment);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}/resend-approve")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResendByIdAsync(decimal id)
        {
            var result = await _iPrmTransactionLogHandler.ResendByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByIdAsync(decimal id)
        {
            var result = await _iPrmTransactionLogHandler.DeleteByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}/waiting-revoke")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> WaitingRevokeByIdAsync(decimal id, string accRevoke)
        {
            string posRevoke = "";
            string userStaffRevoke = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null)
            {
                posRevoke = requestInfo.PosCd;
                userStaffRevoke = requestInfo.UserName;
            }
            var result = await _iPrmTransactionLogHandler.RevokeByIdAsync(id, accRevoke, "WAITING_REVOKE", "", "", posRevoke, userStaffRevoke);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}/approve-revoke")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveRevokeByIdAsync(decimal id, string revokeComment)
        {
            string revokedBy = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) revokedBy = requestInfo.UserName;
            var result = await _iPrmTransactionLogHandler.RevokeByIdAsync(id, "", "REVOKED", revokedBy, revokeComment, "", "");
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}/reject-revoke")]
        [ProducesResponseType(typeof(ResponseObject<PrmTransactionLogModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectRevokeByIdAsync(decimal id, string revokeComment)
        {
            string revokedBy = "";
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) revokedBy = requestInfo.UserName;
            var result = await _iPrmTransactionLogHandler.RevokeByIdAsync(id, "", "REJECTED_REVOKE", revokedBy, revokeComment, "", "");
            return RequestHelpers.TransformData(result);
        }

        //[HttpPut, Route("{id}/{status}")]
        //[ProducesResponseType(typeof(ResponseObject<PrmPromotionModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> UpdateStatusAsync(decimal id, string status)
        //{

        //    var result = await _iPrmPromotionHandler.UpdateStatusAsync(id, status);
        //    return RequestHelpers.TransformData(result);
        //}
        #endregion
    }
}