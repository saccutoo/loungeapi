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
    [Route("api/cms/elgconfigurate")]
    [ApiController]
    public class ElgConfigurateController : ControllerBase
    {
        private readonly IElgConfigurateHandler _interfaceHandler;
        public ElgConfigurateController(IElgConfigurateHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<ElgConfigurateBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgConfigurateQueryModel queryModel = new ElgConfigurateQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgConfigurateQueryModel>(query);

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
        #endregion

        #region CRUD
        [HttpPost]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgConfigurateBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ElgConfigurateCreateUpdateModel model)
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
        [ProducesResponseType(typeof(ResponseObject<ElgConfigurateBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] ElgConfigurateCreateUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateAsync(id, model, baseModel);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}