using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/promo/product")]
    [ApiController]
    public class PrmProductController : ControllerBase
    {
        private readonly IPrmProductHandler _iPrmProductHandler;
        public PrmProductController(IPrmProductHandler iPrmProductHandler)
        {
            _iPrmProductHandler = iPrmProductHandler;
        }
        #region GET

        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByFilter(string query)
        {
            PrmProductQueryModel queryModel = new PrmProductQueryModel();
            Response result = null;

            if (!string.IsNullOrEmpty(query))
            {
                queryModel = JsonConvert.DeserializeObject<PrmProductQueryModel>(query);
                result = await _iPrmProductHandler.GetByFilterAsync(queryModel);
            }
            else result = new ResponseError(Utils.StatusCode.Fail, "Thiếu dữ liệu truy vấn");

            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _iPrmProductHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        #endregion

        #region CRUD
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<PrmProductModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] PrmProductCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.CreateBy = requestInfo.UserName;
            var result = await _iPrmProductHandler.CreateAsync(model);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut,Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id,[FromBody] PrmProductUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.LastModifiedBy = requestInfo.UserName;
            var result = await _iPrmProductHandler.UpdateAsync(id,model);
            return RequestHelpers.TransformData(result);
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByIdAsync(decimal id)
        {

            var result = await _iPrmProductHandler.DeleteByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}