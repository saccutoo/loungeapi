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
    [Route("api/cms/elgbookings")]
    [ApiController]
    public class ElgBookingsController : ControllerBase
    {
        private readonly IElgBookingsHandler _interfaceHandler;
        public ElgBookingsController(IElgBookingsHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET        
        [HttpGet]
        //[Permission(TypeFilter.Jwt, "Admin")]
        [Route("checkbooking")]
        [ProducesResponseType(typeof(ResponseObject<ElgCheckBookingModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckBooking(string query)
        {
            var result = await _interfaceHandler.CheckBookingAsync(query);
            return RequestHelpers.TransformData(result);
        }


        [HttpGet]
        //[Permission(TypeFilter.Jwt, "Admin")]
        [Route("checkbooking_v2")]
        [ProducesResponseType(typeof(ResponseObject<ElgCheckBookingModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckBookingV2(string query)
        {
            var result = await _interfaceHandler.CheckBookingV2Async(query);
            return RequestHelpers.TransformData(result);
        }


        [HttpGet]
        [Route("filter")]
        [Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgBookingsQueryModel queryModel = new ElgBookingsQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgBookingsQueryModel>(query);

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
        [Route("reserver/filter")]
        
        [ProducesResponseType(typeof(ResponseObject<ElgBookingsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReserverBookingFilterAsync(string query)
        {
            ElgBookingsQueryModel queryModel = new ElgBookingsQueryModel();
            Response result = null;
            if (string.IsNullOrEmpty(query))
            {
                queryModel = new ElgBookingsQueryModel()
                {
                    EndDate = DateTime.Now,
                    StartDate = DateTime.Now
                };
            }
            if (!String.IsNullOrEmpty(query))
            {

                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgBookingsQueryModel>(query);

                }
                catch (Exception ex)
                {
                    result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
                }
            }

            result = await _interfaceHandler.GetReserverBookingFilterAsync(queryModel);

            return RequestHelpers.TransformData(result);
        }


        [HttpGet]
        [Route("{id}")]
        //[Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _interfaceHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        //vinhtq:26/05/2020
        [HttpGet]
        [Route("bookingscheckinfilter")]
        [ProducesResponseType(typeof(ResponseObject<List<ElgBookingCheckinModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookingsCheckinFilter(string query)
        {
            ElgCheckInQueryModel queryModel = new ElgCheckInQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgCheckInQueryModel>(query);
                    result = await _interfaceHandler.GetBookingsCheckinFilterAsync(queryModel);
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
        //[Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] ElgBookingsCreateUpdateModel model)
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
        //[Permission(TypeFilter.Jwt, "Admin")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] ElgBookingsCreateUpdateModel model)
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
        //[Permission(TypeFilter.Jwt, "Admin")]
        [Route("{id}/status")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingsBaseModel>), StatusCodes.Status200OK)]
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


        //vinhtq::28/05/2020
        [HttpPut]
        //[Permission(TypeFilter.Jwt, "Admin")]
        [Route("update-behavior")]
        [ProducesResponseType(typeof(ResponseObject<ElgBookingsBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] ElgBookingUpdateBehavior model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var result = await _interfaceHandler.UpdateBookingCustomerBehavior(model.BookingId, model.IsAddBehavior, requestInfo.UserName);
            return RequestHelpers.TransformData(result);
        }
        #endregion


        //vinhtq::08/12/2021
        //Hàm search khách hàng clone theo hàm search v2 của anh tungck
        [HttpGet]
        //[Permission(TypeFilter.Jwt, "Admin")]
        [Route("checkbooking_v3")]
        [ProducesResponseType(typeof(ResponseObject<ElgCheckBookingModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckBookingV3(decimal elgCustId)
        {
            var result = await _interfaceHandler.CheckBookingV3Async(elgCustId);
            return RequestHelpers.TransformData(result);
        }
    }
}