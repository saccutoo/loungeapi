using API.Infrastructure.Repositories;
using API.Interface;
using API.Models;
using Dapper.Oracle;
using API.Infrastructure.Migrations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class ElgLoungeHandler : IElgLoungeHandler
    {
        private readonly RepositoryHandler<ElgLounge, ElgLoungesBaseModel, ElgLoungesQueryModel> _elgLoungeHandler
              = new RepositoryHandler<ElgLounge, ElgLoungesBaseModel, ElgLoungesQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgLoungeHandler> _logger;

        public ElgLoungeHandler(ILogger<ElgLoungeHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetAllListAsync()
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_LOUNGE.PRC_GET_LIST_LOUNGES");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", string.Empty, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgLoungeHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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
