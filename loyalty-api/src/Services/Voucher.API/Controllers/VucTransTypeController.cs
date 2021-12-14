using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voucher.API.Interface;
using Voucher.API.Models;
using System.Threading.Tasks;
using Utils;
using API.Filters;
using Newtonsoft.Json;

namespace Voucher.API.Controllers
{
    [Route("api/cms/vuctranstype")]
    [ApiController]
    public class VucTransTypeController : ControllerBase
    {
        private readonly IVucTransTypeHandler _interfaceHandler;
        public VucTransTypeController(IVucTransTypeHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherTypeBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListTransType(string query)
        {
            var result = await _interfaceHandler.GetListTransType(query);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}