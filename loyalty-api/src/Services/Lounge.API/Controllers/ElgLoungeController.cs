using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interface;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utils;

namespace API.Controllers
{
    [Route("api/cms/[controller]")]
    [ApiController]
    public class ElgLoungeController : ControllerBase
    {
        private readonly IElgLoungeHandler _interfaceHandler;
        public ElgLoungeController(IElgLoungeHandler interfaceHandler)
        {
            _interfaceHandler = interfaceHandler;
        }

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(typeof(ResponseObject<ElgLoungesBaseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllList()
        {
            var result = await _interfaceHandler.GetAllListAsync();
            return RequestHelpers.TransformData(result);
        }
    }
}