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
    public class SrvSurveySectionsHandler : ISrvSurveySectionsHandler
    {
        private readonly RepositoryHandler<SrvSurveySections, SrvSurveySections, ElgReviewQuality> _srvhandler
               = new RepositoryHandler<SrvSurveySections, SrvSurveySections, ElgReviewQuality>();
        private string _dBSchemaName;
        private readonly ILogger<SrvSurveySectionsHandler> _logger;

        public SrvSurveySectionsHandler(ILogger<SrvSurveySectionsHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetSurveySectionsAsync(decimal loungeId)
        {
            try
            {
                _logger.LogInformation("SrvSurveySections - GetSurveySectionsAsync - REQ: " + loungeId);

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_SURVEY_SECTIONS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_lounge_id", loungeId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _srvhandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("SrvSurveySections - GetSurveySectionsAsync - RES:" + JsonConvert.SerializeObject(result));

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
