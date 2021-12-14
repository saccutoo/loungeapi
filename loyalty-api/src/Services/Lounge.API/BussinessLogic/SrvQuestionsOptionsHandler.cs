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
    public class SrvQuestionsOptionsHandler : ISrvQuestionsOptionsHandler
    {
        private readonly RepositoryHandler<SrvQuestionsOptions, SrvQuestionsOptions, ElgReviewQuality> _srvhandler
               = new RepositoryHandler<SrvQuestionsOptions, SrvQuestionsOptions, ElgReviewQuality>();
        private string _dBSchemaName;
        private readonly ILogger<SrvQuestionsOptionsHandler> _logger;

        public SrvQuestionsOptionsHandler(ILogger<SrvQuestionsOptionsHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetQuestionsOptionsAsync(decimal questionId)
        {
            try
            {
                _logger.LogInformation("SrvQuestionsOptions - GetQuestionsOptionsAsync - REQ: " + questionId);

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_QUESTION_OPTIONS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_question_id", questionId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _srvhandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("SrvQuestionsOptions - GetQuestionsOptionsAsync - RES:" + JsonConvert.SerializeObject(result));

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
