using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/promo/cost-account")]
    [ApiController]
    public class PrmCostAccountController : ControllerBase
    {
        private readonly IPrmCostAccountHandler _iPrmCostAccountHandler;
        public PrmCostAccountController(IPrmCostAccountHandler iPrmCostAccountHandler)
        {
            _iPrmCostAccountHandler = iPrmCostAccountHandler;
        }

        #region GET

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<PrmCostAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            PrmCostAccountQueryModel queryModel = new PrmCostAccountQueryModel();
            Response result = null;

            if (!string.IsNullOrEmpty(query))
            {
                queryModel = JsonConvert.DeserializeObject<PrmCostAccountQueryModel>(query);
                result = await _iPrmCostAccountHandler.GetByFilterAsync(queryModel);
            }
            else result = new ResponseError(Utils.StatusCode.Fail, "Thiếu dữ liệu truy vấn");

            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmCostAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _iPrmCostAccountHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion

        #region CRUD
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<PrmCostAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] PrmCostAccountCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.CreatedBy = requestInfo.UserName;
            var result = await _iPrmCostAccountHandler.CreateAsync(model);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut,Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmCostAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id,[FromBody] PrmCostAccountUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.LastModifiedBy = requestInfo.UserName;
            var result = await _iPrmCostAccountHandler.UpdateAsync(id,model);
            return RequestHelpers.TransformData(result);
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmCostAccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByIdAsync(decimal id)
        {

            var result = await _iPrmCostAccountHandler.DeleteByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}