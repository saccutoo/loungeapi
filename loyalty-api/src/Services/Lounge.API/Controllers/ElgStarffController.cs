using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using API.Filters;
using Newtonsoft.Json;
using System;

namespace API.Controllers
{
    [Route("api/cms/elgstarff")]
    [ApiController]
    public class ElgStarffController : ControllerBase
    {
        private readonly IElgStarffHandler _interfaceHandler;
        public ElgStarffController(IElgStarffHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgStarffQueryModel queryModel = new ElgStarffQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgStarffQueryModel>(query);

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
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCondition(string query)
        {
            var result = await _interfaceHandler.GetByAllListAsync(query);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _interfaceHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpPost]
        [Route("login-jwt")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarfLoginResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginJWT([FromBody]ElgStarfLoginModel loginModel)
        {
            var result = await _interfaceHandler.LoginJWT(loginModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Permission(TypeFilter.Jwt, "Admin")]
        [Route("{id}/logout")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout(decimal id)
        {
            var result = await _interfaceHandler.Logout(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Permission(TypeFilter.Jwt, "Admin")]
        [Route("change-pass/{id}")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword(decimal id, [FromBody]ElgStarfChangePasswordModel changePassModel)
        {
            var result = await _interfaceHandler.ChangePassword(id, changePassModel.OldPassword, changePassModel.NewPassword);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("change-pass/{id}/force")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ForceChangePassword(decimal id, [FromBody]ElgStarfChangePasswordModel changePassModel)
        {
            var result = await _interfaceHandler.ChangePassword(id, changePassModel.NewPassword, changePassModel.NewPassword, true);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("refresh-token")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarfLoginResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken(string token)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var permissionToken = "";
            if (requestInfo != null)
            {
                permissionToken = requestInfo.PermissionToken;
            }
            var result = await _interfaceHandler.RefreshToken(permissionToken, token);
            return RequestHelpers.TransformData(result);
        }

        #endregion

        #region CRUD
        [HttpPost]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ElgStarffCreateModel model)
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
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] ElgStarffUpdateModel model)
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
        [ProducesResponseType(typeof(ResponseObject<ElgStarffBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatus(decimal id, string status)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateStatusAsync(id, status, baseModel);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}