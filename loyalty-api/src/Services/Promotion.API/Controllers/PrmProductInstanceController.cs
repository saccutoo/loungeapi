using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/promo/product-instance")]
    [ApiController]
    public class PrmProductInstanceController : ControllerBase
    {
        private readonly IPrmProductInstanceHandler _iPrmProductInstanceHandler;
        public PrmProductInstanceController(IPrmProductInstanceHandler iPrmProductInstanceHandler)
        {
            _iPrmProductInstanceHandler = iPrmProductInstanceHandler;
        }
        #region GET

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductInstanceModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(decimal id)
        {
            var result = await _iPrmProductInstanceHandler.GetByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("{id}/by-promotion")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductInstanceModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPromotionId(decimal id)
        {
            var result = await _iPrmProductInstanceHandler.GetByPromotionIdAsync(id);
            return RequestHelpers.TransformData(result);
        }

        #endregion

        #region CRUD
        [HttpPost]
        [ProducesResponseType(typeof(ResponseObject<PrmProductInstanceModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAsync([FromBody] PrmProductInstanceCreateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.CreateBy = requestInfo.UserName;
            var result = await _iPrmProductInstanceHandler.CreateAsync(model);
            return RequestHelpers.TransformData(result);
        }

        [HttpPut, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductInstanceModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(decimal id, [FromBody] PrmProductInstanceUpdateModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            if (requestInfo != null) model.LastModifiedBy = requestInfo.UserName;
            var result = await _iPrmProductInstanceHandler.UpdateAsync(id, model);
            return RequestHelpers.TransformData(result);
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<PrmProductInstanceModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByIdAsync(decimal id)
        {

            var result = await _iPrmProductInstanceHandler.DeleteByIdAsync(id);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}