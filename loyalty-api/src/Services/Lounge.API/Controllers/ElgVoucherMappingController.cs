using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using API.Filters;
using Newtonsoft.Json;
using System;
using Lounge.API.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/cms/elgvouchermapping")]
    [ApiController]
    public class ElgVoucherMappingController : ControllerBase
    {
        private readonly IElgVoucherMappingHandler _interfaceHandler;

        private readonly ILogger<ElgVoucherMappingController> _logger;
        public ElgVoucherMappingController(IElgVoucherMappingHandler interfaceHandler, ILogger<ElgVoucherMappingController> logger)
        {
            _interfaceHandler = interfaceHandler;
            _logger = logger;
        }
        #region GET

        [HttpGet]
        [Route("list/uploadid/approved")]
        public async Task<IActionResult> GetApprovedUploadIds(string templateIds, int pageSize, int pageIndex)
        {
            var result = await _interfaceHandler.GetApprovedUploadIds(templateIds, pageSize, pageIndex);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("list/upload-to-approved")]
        public async Task<IActionResult> GetFileUploadsToApproved(string pos)
        {
            if (pos == "110000")
            {
                pos = "";
            }
            var result = await _interfaceHandler.GetFileUploads(pos);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("filter")]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgVoucherMappingQueryModel queryModel = null;
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgVoucherMappingQueryModel>(query);
                    //if (queryModel.PosId > 0)
                    //{
                    //    if (queryModel.PosId == 110000)
                    //    {
                    //        queryModel.PosId = 0;
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
                }
            }
            if(queryModel == null)
            {
                queryModel = new ElgVoucherMappingQueryModel();                
            }
            result = await _interfaceHandler.GetByFilterAsync(queryModel);

            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("get-cust-by-cif")]
        public async Task<IActionResult> GetCustByCif(string cif)
        {
            Response result = null;

            if (string.IsNullOrEmpty(cif))
            {
                return null;
            }
            result = await _interfaceHandler.GetCustByCif(cif);

            return RequestHelpers.TransformData(result);
        }


        #endregion

        #region CRUD


        [HttpPost]
        [Route("map/customer")]
        public async Task<IActionResult> MapCustVoucher(VucMappingVoucherCustModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                LastModifyBy = requestInfo.UserName
            };
            var result = await _interfaceHandler.MappingVoucherCustomer(model, baseModel);
            return RequestHelpers.TransformData(result);
        }

        [HttpPost]
        [Route("map-manual/customer")]
        public async Task<IActionResult> MapManualCustVoucher(VucMappingManualVoucherCustModel model)
        {
            
            _logger.LogDebug("MapManualCustVoucher model: " + JsonConvert.SerializeObject(model));
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new EVoucherBaseModel
            {
                CreateBy = requestInfo.UserName,
                LastModifyBy = requestInfo.UserName
            };
            var resultVoucherMapping = await _interfaceHandler.CreateAsync(model, baseModel) as ResponseObject<ResponseModel>; 
            if(resultVoucherMapping.StatusCode == Utils.StatusCode.Success)
            {
                var vouCustId = resultVoucherMapping.Data.Id;
                var result = await _interfaceHandler.MappingVoucherCustomer(new VucMappingVoucherCustModel() {
                    TransType = "0",
                    UploadId = "0",
                    VoucherId = model.VoucherId,                    
                    ListCustomer = new List<CustVoucherModel>()
                    {
                        new CustVoucherModel()
                        {
                            CIF = model.CIF,
                            CustomerId = model.CustomerId,
                            CustomerName = model.CustomerName,
                            VouCustomerId = vouCustId,
                            MaxUsedPerCust = model.MaxUsedPerCust,
                            Pos = model.Pos
                        }
                    }

                }, baseModel);

                return RequestHelpers.TransformData(result);
            }
            
            return RequestHelpers.TransformData(resultVoucherMapping);
        }

        [HttpPut]
        [Route("approved")]
        public async Task<IActionResult> Approved(string listIds, string username,string pos, decimal uploadId)
        {
            Response result = null;
            if (pos == "110000")
            {
                pos = "";
            }
            result = await _interfaceHandler.Approved(listIds, username, pos, uploadId);

            return RequestHelpers.TransformData(result);
        }

        [HttpPut]
        [Route("approved/byuploadid")]
        public async Task<IActionResult> ApprovedByUploadId(string username, string pos, decimal uploadId)
        {
            Response result = null;

            result = await _interfaceHandler.Approved("",username, pos, uploadId);

            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("voucher/filter")]
        public async Task<IActionResult> GetMapVoucherListByFilter(string query)
        {
            VucMapVoucherCustQueryModel queryModel = new VucMapVoucherCustQueryModel();
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<VucMapVoucherCustQueryModel>(query);
                    if (!string.IsNullOrEmpty(queryModel?.Pos))
                    {
                        if(queryModel.Pos == "110000")
                        {
                            queryModel.Pos = string.Empty;
                        }
                    }
                    result = await _interfaceHandler.GetMapVoucherListByFilter(queryModel);
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