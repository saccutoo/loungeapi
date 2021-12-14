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
    [Route("api/cms/elgbookingcheckout")]
    [ApiController]
    public class ElgBookingCheckoutController : ControllerBase
    {
        private readonly IElgBookingCheckoutHandler _interfaceHandler;
        public ElgBookingCheckoutController(IElgBookingCheckoutHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        #region GET

        [HttpGet]
        [Route("filter")]
        [Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingCheckoutBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgBookingCheckoutQueryModel queryModel = new ElgBookingCheckoutQueryModel();
            Response result;
            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgBookingCheckoutQueryModel>(query);

                    result = await _interfaceHandler.GetByFilterAsync(queryModel);
                }
                catch (Exception)
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
        [Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingCheckoutBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(string query)
        {
            Response result;
            try
            {
                result = await _interfaceHandler.GetAllAsync(query);
            }
            catch (Exception)
            {
                result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
            }

            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingCheckoutBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Checkout([FromBody] ElgCheckOutCifsModel cifs)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                CreateBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.CheckoutAsync(cifs, baseModel);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}