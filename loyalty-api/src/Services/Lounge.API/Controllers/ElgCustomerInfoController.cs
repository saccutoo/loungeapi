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
    [Route("api/cms/elgcustomer-info")]
    [ApiController]
    public class ElgCustomerInfoController : ControllerBase
    {
        private readonly IElgCustomerInfoHandler _interfaceHandler;
        public ElgCustomerInfoController(IElgCustomerInfoHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerInfoModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgCustomerInfoQueryModel queryModel = new ElgCustomerInfoQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgCustomerInfoQueryModel>(query);

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
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _interfaceHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

       
        #endregion

        #region CRUD
       
        [HttpPut]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerInfoModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] ElgCustomerInfoUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                LastModifiedBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }       
        #endregion
    }
}