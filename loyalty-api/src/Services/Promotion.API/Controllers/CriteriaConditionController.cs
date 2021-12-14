using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Interface;
using API.Models;
using System.Threading.Tasks;
using Utils;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/criteria-condition")]
    [ApiController]
    public class CriteriaConditionController : ControllerBase
    {
        private readonly ICriteriaConditionHandler _iCriteriaConditionHandler;
        public CriteriaConditionController(ICriteriaConditionHandler iCriteriaConditionHandler)
        {
            _iCriteriaConditionHandler = iCriteriaConditionHandler;
        }

        #region GET
 
        [HttpGet]
        [Route("criteria/all-active")]
        [ProducesResponseType(typeof(ResponseObject<CriteriaModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCriteriaActive()
        {
            var result = await _iCriteriaConditionHandler.GetAllCriteriaActive();
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("condition/{code}/by-criteria")]
        [ProducesResponseType(typeof(ResponseObject<CriteriaModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConditionByCriteria(string code)
        {
            var result = await _iCriteriaConditionHandler.GetConditionByCriteria(code);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}