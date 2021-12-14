using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;
using Voucher.API.Infrastructure.Migrations;
using Voucher.API.Interface;

namespace Voucher.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VucVoucherAmtConditionController : ControllerBase
    {
        private readonly IVucVoucherAmtConditionsHandler _interfaceHandler;
        public VucVoucherAmtConditionController(IVucVoucherAmtConditionsHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        #region GET        
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResponseObject<VucVoucherAmtConditions>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVoucherAmtConditionsByVoucherID(decimal id)
        {
            var result = await _interfaceHandler.GetVoucherConditionsAll(id);
            return Utils.RequestHelpers.TransformData(result);
        }
        #endregion
    }
}