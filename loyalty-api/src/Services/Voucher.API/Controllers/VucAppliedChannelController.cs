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
    [Route("api/cms/appliedchannel")]
    [ApiController]
    public class VucAppliedChannelController : ControllerBase
    {
        private readonly IVucAppliedChannelHandler _interfaceHandler;
        public VucAppliedChannelController(IVucAppliedChannelHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<VucAppliedChannelBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListChannel(string query)
        {
            var result = await _interfaceHandler.GetListVoucherChannel(query);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}