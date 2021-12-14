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
    [Route("api/cms/elgreport")]
    [ApiController]
    public class ElgReportController : ControllerBase
    {
        private readonly IElgReportHandler _interfaceHandler;
        public ElgReportController(IElgReportHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        #region GET
        //[HttpGet]
        //[Route("customer/list")]
        //[ProducesResponseType(typeof(ResponseObject<ElgReportCustomerModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetListCustomer(string query)
        //{
        //    ElgReportQueryModel queryModel = new ElgReportQueryModel();
        //    Response result = null;

        //    if (!String.IsNullOrEmpty(query))
        //    {
        //        try
        //        {
        //            queryModel = JsonConvert.DeserializeObject<ElgReportQueryModel>(query);

        //            result = await _interfaceHandler.GetListCustomerAsync(queryModel);
        //        }
        //        catch (Exception ex)
        //        {
        //            result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
        //        }
        //    }
        //    else
        //    {
        //        result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
        //    }

        //    return RequestHelpers.TransformData(result);
        //}

        [HttpGet]
        [Route("customer/export")]
        [ProducesResponseType(typeof(ResponseObject<ElgReportCustomerModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportListCustomer(string query)
        {
            ElgReportQueryModel queryModel = new ElgReportQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgReportQueryModel>(query);

                    result = await _interfaceHandler.ExportListCustomerAsync(queryModel);
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

        //[HttpGet]
        //[Route("booking/list")]
        //[ProducesResponseType(typeof(ResponseObject<ElgReportBookingModel>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetListBooking(string query)
        //{
        //    ElgReportQueryModel queryModel = new ElgReportQueryModel();
        //    Response result = null;

        //    if (!String.IsNullOrEmpty(query))
        //    {
        //        try
        //        {
        //            queryModel = JsonConvert.DeserializeObject<ElgReportQueryModel>(query);

        //            result = await _interfaceHandler.GetListBookingAsync(queryModel);
        //        }
        //        catch (Exception ex)
        //        {
        //            result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
        //        }
        //    }
        //    else
        //    {
        //        result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
        //    }

        //    return RequestHelpers.TransformData(result);
        //}

        [HttpGet]
        [Route("booking/export")]
        [ProducesResponseType(typeof(ResponseObject<ElgReportBookingModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportListBooking(string query)
        {
            ElgReportQueryModel queryModel = new ElgReportQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgReportQueryModel>(query);

                    result = await _interfaceHandler.ExportListBookingAsync(queryModel);
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
        [Route("dashboard/export")]
        [ProducesResponseType(typeof(ResponseObject<ElgReportBookingModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportDashboardAsync(string query)
        {
            ElgReportQueryModel queryModel = new ElgReportQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgReportQueryModel>(query);

                    result = await _interfaceHandler.ExportDashboardAsync(queryModel);
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
        [Route("summary/export")]
        [ProducesResponseType(typeof(ResponseObject<ElgReportBookingModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportSummaryBooking(string query)
        {
            ElgReportQueryModel queryModel = new ElgReportQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgReportQueryModel>(query);

                    result = await _interfaceHandler.ExportSummaryBookingAsync(queryModel);
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
        [Route("accompanyingperson/export")]
        [ProducesResponseType(typeof(ResponseObject<ElgCheckinPeopleGoWidthBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportAccompanyingPersonAsync(string query)
        {
            ElgCheckinPeopleGoWidthQueryModel queryModel = new ElgCheckinPeopleGoWidthQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgCheckinPeopleGoWidthQueryModel>(query);

                    result = await _interfaceHandler.ExportElgCheckinPeopleGoWidthAsync(queryModel);
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
    }
}