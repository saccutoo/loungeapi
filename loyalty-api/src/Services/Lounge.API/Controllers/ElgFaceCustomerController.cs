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
    [Route("api/cms/elgfacecustomer")]
    [ApiController]
    public class ElgFaceCustomerController : ControllerBase
    {
        private readonly IElgFaceCustomerHandler _interfaceHandler;
        public ElgFaceCustomerController(IElgFaceCustomerHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET   

        [HttpGet]
        [Route("all")]
        [ProducesResponseType(typeof(ResponseObject<ElgFaceCustomerViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByTypeAsync(string type)
        {
            var result = await _interfaceHandler.GetAllByTypeAsync(type);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("faceid")]
        [ProducesResponseType(typeof(ResponseObject<ElgUploadId>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFaceIdAsync(string faceId)
        {
            var result = await _interfaceHandler.GetByFaceIdAsync(faceId);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<ElgFaceCustomerViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _interfaceHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

      
        #endregion

        #region CRUD
        [HttpPost]
        //[Permission(TypeFilter.CheckPermission, "CREATE_UNG_DUNG")]
        [ProducesResponseType(typeof(ResponseObject<ElgFaceCustomerViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ElgFaceCustomerCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            model.CreateBy = requestInfo.UserName;
            var result = await _interfaceHandler.CreateAsync(model);
            return RequestHelpers.TransformData(result);
        }

        
        #endregion
     

    }
}