using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/status-config")]
    [ApiController]
    public class StatusConfigController : ControllerBase
    {
        private readonly IStatusConfigHandler _iStatusConfigHandler;
        public StatusConfigController(IStatusConfigHandler iStatusConfigHandler)
        {
            _iStatusConfigHandler = iStatusConfigHandler;
        }

        #region GET
 
        [HttpGet]
        [Route("all-active")]
        [ProducesResponseType(typeof(ResponseObject<StatusConfigModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _iStatusConfigHandler.GetAllActive();
            return RequestHelpers.TransformData(result);
        }
        
        #endregion
    }
}