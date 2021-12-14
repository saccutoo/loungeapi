using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voucher.API.Interface;
using Voucher.API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;
using System;

namespace Voucher.API.Controllers
{
    [Route("api/cms/vuc-customer")]
    [ApiController]
    public class VucCustomerController : ControllerBase
    {
        private readonly IVucCustomerHandler _interfaceHandler;
        public VucCustomerController(IVucCustomerHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        #region GET
        [HttpGet]
        [Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<VucCustomerModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerByFilter(string filterModel)
        {         
            VucCustomerQueryModel query= JsonConvert.DeserializeObject<VucCustomerQueryModel>(filterModel);
            var result = await _interfaceHandler.GetCustomerByFilter(query);
            return RequestHelpers.TransformData(result);
        }      
        #endregion      
    }
}