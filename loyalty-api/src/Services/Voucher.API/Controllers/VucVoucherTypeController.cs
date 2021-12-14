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
    [Route("api/cms/vouchertype")]
    [ApiController]
    public class VucVoucherTypeController : ControllerBase
    {
        private readonly IVucVoucherTypeHandler _interfaceHandler;
        public VucVoucherTypeController(IVucVoucherTypeHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherTypeBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListType(string query)
        {
            var result = await _interfaceHandler.GetListVoucherType(query);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}