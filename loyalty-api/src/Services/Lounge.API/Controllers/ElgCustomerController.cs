using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using API.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using API.Infrastructure.Migrations;

namespace API.Controllers
{
    [Route("api/cms/elgcustomer")]
    [ApiController]
    public class ElgCustomerController : ControllerBase
    {
        private readonly IElgCustomerHandler _interfaceHandler;
        public ElgCustomerController(IElgCustomerHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgCustomerQueryModel queryModel = new ElgCustomerQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgCustomerQueryModel>(query);

                    result = await _interfaceHandler.GetByFilterAsync(queryModel);
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
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByCondition(string query)
        {
            var result = await _interfaceHandler.GetAllByConditionAsync(query);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("list/uploadids")]
        [ProducesResponseType(typeof(ResponseObject<ElgUploadId>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterByUploadIds(decimal uploadid1, decimal uploadid2)
        {
            var result = await _interfaceHandler.FilterByUploadIds(uploadid1, uploadid2);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _interfaceHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("list/uploadid/approved")]
        [ProducesResponseType(typeof(ResponseObject<ElgUploadId>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApprovedUploadIds()
        {
            var result = await _interfaceHandler.GetApprovedUploadIds();
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("list/uploadid/pending")]
        [ProducesResponseType(typeof(ResponseObject<ElgUploadId>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingUploadIds()
        {
            var result = await _interfaceHandler.GetPendingUploadIds();
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("export")]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportCustomer(string query)
        {
            ElgCustomerExportModel queryModel = new ElgCustomerExportModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgCustomerExportModel>(query);

                    result = await _interfaceHandler.ExportCustomerAsync(queryModel);
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
        [Route("get-customer-by-id")]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetElgCustomerById(decimal elgId)
        {
            Response result = null;

            if (elgId!=0)
            {
                try
                {
                    result = await _interfaceHandler.GetElgCustomerById(elgId);
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
        [Route("getbycustid")]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetElgCustomerByCustId(string custId)
        {
            Response result = null;

            if (!string.IsNullOrEmpty(custId))
            {
                try
                {
                    result = await _interfaceHandler.GetByCustIdAsync(custId);
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

        #region CRUD
        [HttpPost]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ElgCustomerCreateUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                CreateBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.CreateAsync(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        //[Route("{id}")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerClassBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] ElgCustomerCreateUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateAsync(id, model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("{id}/status")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, string status)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateStatusAsync(id, status, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("trans/{uploadid}/status")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUploadIDStatusAsync(decimal uploadid, string status)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateUploadIDStatusAsync(uploadid, status, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("trans/{old_uploadid}/{new_uploadid}/status")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMultiUploadIDStatusAsync(decimal old_uploadid, decimal new_uploadid, string status)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateMultiUploadIDStatusAsync(old_uploadid, new_uploadid, status, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("{id}/conflict/resolve")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResolveConflictAsync(decimal id, [FromBody] ElgCustomerResolveConflictModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.ResolveConflictAsync(id, model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        //vinhtq1

        [HttpPut]
        [Route("update-detail")]
        //[Permission(TypeFilter.CheckPermission, "UPDATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDetailCustomer([FromBody] ElgCustomerBaseModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.CreateBy = requestInfo.UserName;
            var result = await _interfaceHandler.UpdateDetailCustomer(model);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        //vinhtq1:07/12/2021
        //Hàm search danh sách khách hàng và không kiểm tra điều kiện voucher hoặc là còn được checkin hay booking ko.

        [HttpGet]
        [Route("listcustomerv3")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListCustomerV3Async(string textSearch, string fullName, string phoneNum, string cusname, string representUserName, string email,string status)
        {
            Response result = null;
            try
            {
                result = await _interfaceHandler.GetListCustomerV3Async(textSearch, fullName, phoneNum, cusname, representUserName, email,status);
            }
            catch (Exception ex)
            {
                result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
            }

            return RequestHelpers.TransformData(result);
        }

    }
}