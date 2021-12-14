using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using API.Interface;
using API.Models;
using API.Infrastructure.Migrations;
using Newtonsoft.Json;

namespace API.BussinessLogic
{
    public class SrvSurveyQuestionsHandler : ISrvSurveyQuestionsHandler
    {
        private readonly RepositoryHandler<SrvSurveyQuestions, SrvSurveyQuestions, ElgReviewQuality> _srvhandler
               = new RepositoryHandler<SrvSurveyQuestions, SrvSurveyQuestions, ElgReviewQuality>();
        private string _dBSchemaName;
        private readonly ILogger<SrvSurveyQuestionsHandler> _logger;

        public SrvSurveyQuestionsHandler(ILogger<SrvSurveyQuestionsHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetSurveyQuestionsAsync(decimal surveySectionId)
        {
            try
            {
                _logger.LogInformation("SrvSurveyQuestions - GetSurveyQuestionsAsync - REQ: " + surveySectionId);

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_SURVEY_QUESTIONS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_survey_section_id", surveySectionId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _srvhandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("SrvSurveyQuestions - GetSurveyQuestionsAsync - RES:" + JsonConvert.SerializeObject(result));

                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                else throw ex;
            }
        }
        
    }
}
