using API.Infrastructure.Migrations;
using API.Interface;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Utils;

namespace API.Controllers
{
    [Route("api/cms/elgreviewquality")]
    [ApiController]
    public class ElgReviewQualityController : ControllerBase
    {
        //private readonly ISrvQuestionsOptionsHandler _srvQuestionsOptionsHandler;
        //private readonly ISrvSurveyQuestionsHandler _srvSurveyQuestionsler;
        //private readonly ISrvSurveySectionsHandler _srvSurveySectionsHandler;
        private readonly IElgReviewQualityHandler _elgReviewQualityHandler;

        public ElgReviewQualityController(
            //ISrvQuestionsOptionsHandler srvQuestionsOptionsHandler,
            //ISrvSurveyQuestionsHandler srvSurveyQuestionsler
            //, ISrvSurveySectionsHandler srvSurveySectionsHandler,
            IElgReviewQualityHandler elgReviewQualityHandler)
        {
            //_srvQuestionsOptionsHandler = srvQuestionsOptionsHandler;
            //_srvSurveyQuestionsler = srvSurveyQuestionsler;
            //_srvSurveySectionsHandler = srvSurveySectionsHandler;
            _elgReviewQualityHandler = elgReviewQualityHandler;

        }
        #region GET
        //[HttpGet]
        //[Route("questionspoptions")]
        //[ProducesResponseType(typeof(ResponseObject<SrvQuestionsOptions>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetQuestionsOptions(decimal questionId)
        //{
        //    var result = await _srvQuestionsOptionsHandler.GetQuestionsOptionsAsync(questionId);
        //    return RequestHelpers.TransformData(result);
        //}
        //[HttpGet]
        //[Route("surveyquestions")]
        //[ProducesResponseType(typeof(ResponseObject<SrvSurveyQuestions>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetSurveyQuestions(decimal surveySectionId)
        //{
        //    var result = await _srvSurveyQuestionsler.GetSurveyQuestionsAsync(surveySectionId);
        //    return RequestHelpers.TransformData(result);
        //}
        [HttpGet]
        [Route("questions")]
        [ProducesResponseType(typeof(ResponseObject<SrvReviewQualityQuestionsModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSurveyQuestions(decimal loungeId, decimal surveySectionId)
        {
            var result = await _elgReviewQualityHandler.GetSurveyQuestions(loungeId, surveySectionId);
            return RequestHelpers.TransformData(result);
        }

        [HttpGet]
        [Route("Report")]
        [ProducesResponseType(typeof(ResponseObject<SrvReviewQualityQuestionsModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Report(string query)
        {
            ElgReportQueryModel queryModel = new ElgReportQueryModel();
            Response result = null;
            try
            {
                queryModel = JsonConvert.DeserializeObject<ElgReportQueryModel>(query);
                result = await _elgReviewQualityHandler.GetReportData(queryModel.FromDate.Value,queryModel.ToDate.Value);

            }
            catch (Exception ex)
            {

                throw;
            }
            return RequestHelpers.TransformData(result);
        }

        //21/05/2021:vinhtq 
        [HttpGet]
        [Route("question_option_choices")]
        [ProducesResponseType(typeof(ResponseObject<SrvQuestionsOptions>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuestionOptions(decimal questionId)
        {
            var result = await _elgReviewQualityHandler.GetQuestionAnswers(questionId);
            return RequestHelpers.TransformData(result);
        }

        #endregion
        #region CRUD
        [HttpPost]
        [Route("reviewquality")]
        [ProducesResponseType(typeof(ResponseObject<ElgReviewQualityModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync([FromBody] ElgReviewQualityModel model)
        {
            var requestInfo = RequestHelpers.GetRequestInfo(Request);
            var baseModel = new ELoungeBaseModel
            {
                CreateBy = requestInfo.UserName
            };
            var result = await _elgReviewQualityHandler.ReviewQualityAsync(model, baseModel);
            return RequestHelpers.TransformData(result);
        }
        #endregion
    }
}