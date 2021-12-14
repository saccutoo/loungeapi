using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using API.Filters;
using Newtonsoft.Json;
using System;

namespace API.Controllers
{
    [Route("api/cms/elgcustomertype")]
    [ApiController]
    public class ElgCustomerTypeController : ControllerBase
    {
        private readonly IElgCustomerTypeHandler _interfaceHandler;
        public ElgCustomerTypeController(IElgCustomerTypeHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }
        #region GET
        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<ElgCustomerTypeBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByCondition(string query)
        {
            var result = await _interfaceHandler.GetAllByConditionAsync(query);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}