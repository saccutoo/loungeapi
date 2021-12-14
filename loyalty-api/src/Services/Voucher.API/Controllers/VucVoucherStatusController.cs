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
    [Route("api/cms/voucherstatus")]
    [ApiController]
    public class VucVoucherStatusController : ControllerBase
    {
        private readonly IVucVoucherStatusHandler _interfaceHandler;
        public VucVoucherStatusController(IVucVoucherStatusHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherStatusBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListStatus(string query)
        {
            var result = await _interfaceHandler.GetListStatus(query);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}