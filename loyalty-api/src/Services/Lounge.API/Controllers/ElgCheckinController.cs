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

namespace API.Controllers
{
    [Route("api/cms/elgcheckin")]
    [ApiController]
    public class ElgCheckinController : ControllerBase
    {
        private readonly IElgCheckinHandler _interfaceHandler;
        public ElgCheckinController(IElgCheckinHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        #region CRUD
        //[HttpPost]
        //[Permission(TypeFilter.Jwt, "Admin")]
        //[ProducesResponseType(typeof(ResponseObject<ElgCheckinPeopleGoWithBaseModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> CreateAsync([FromBody] ElgCheckinPeopleGoWithCreateModel model)
        //{
        //    var result = await _interfaceHandler.CreateAsync(model);
        //    return RequestHelpers.TransformData(result);
        //}

        //[HttpPut]
        //[Permission(TypeFilter.Jwt, "Admin")]
        //[Route("checkin")]
        //[ProducesResponseType(typeof(ResponseObject<ElgCheckinPeopleGoWithBaseModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> CheckinAsync([FromBody] ElgCheckinPeopleGoWithCheckinModel model)
        //{
        //    var requestInfo = RequestHelpers.GetRequestInfo(Request);
        //    var baseModel = new ELoungeBaseModel
        //    {
        //        CreateBy = requestInfo.UserName
        //    };
        //    var result = await _interfaceHandler.CheckinAsync(model, baseModel);
        //    return RequestHelpers.TransformData(result);
        //}

        [HttpPost]
        [Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgCheckinPeopleGoWithBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckinAsync([FromBody] ElgCheckinPeopleGoWithCheckinModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                CreateBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.CheckinAsync(model, baseModel);
            
            return RequestHelpers.TransformData(result);
        }

        [HttpPost]
        [Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgCheckinPeopleGoWithBaseModel>), StatusCodes.Status200OK)]
        [Route("checkin_new")]
        public async Task<IActionResult> CheckinNewAsync([FromBody] ElgCheckinPeopleGoWithCheckinModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                CreateBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.CheckinNewAsync(model, baseModel);

            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region GET

        [HttpGet]
        [Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgCheckinPeopleGoWithBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListCheckInPeopleGoWith(decimal bookingId)
        {
            var result = await _interfaceHandler.GetListCheckInPeopleGoWith(bookingId);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}