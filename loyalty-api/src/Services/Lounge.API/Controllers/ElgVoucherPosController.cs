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

namespace API.Controllers
{
    [Route("api/cms/elgvoucherpos")]
    [ApiController]
    public class ElgVoucherPosController : ControllerBase
    {
        private readonly IElgVoucherPosHandler _interfaceHandler;
        public ElgVoucherPosController(IElgVoucherPosHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<ElgVoucherPosModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            ElgVoucherPosQueryModel queryModel = null;
            Response result = null;

            if (!String.IsNullOrEmpty(query))
            {
                try
                {
                    queryModel = JsonConvert.DeserializeObject<ElgVoucherPosQueryModel>(query);
                }
                catch (Exception ex)
                {
                    result = new ResponseError(Utils.StatusCode.Fail, "Lỗi ngoại lệ");
                }
            }
            if(queryModel == null)
            {
                queryModel = new ElgVoucherPosQueryModel();
            }
            result = await _interfaceHandler.GetByFilterAsync(queryModel);

            return RequestHelpers.TransformData(result);
        }


        #endregion

        #region CRUD
        [HttpPut]
        [Route("UpdateStatus")]
        [ProducesResponseType(typeof(ResponseObject<ElgVoucherPosModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatus(ElgVoucherPosChangeStausModel stausModel)
        {
            Response result = null;
            
            result = await _interfaceHandler.UpdateStatusAsync(stausModel);

            return RequestHelpers.TransformData(result);
        }


        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<ElgVoucherPosModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(ElgVoucherPosCreateModel createModel)
        {
            Response result = null;

            result = await _interfaceHandler.CreateAsync(createModel);

            return RequestHelpers.TransformData(result);
        }

        #endregion
    }
}