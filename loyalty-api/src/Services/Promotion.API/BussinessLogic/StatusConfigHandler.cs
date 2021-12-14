using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using Utils;
using API.Interface;
using API.Models;
using API.Infrastructure.Migrations;
using System.Collections.Generic;

namespace API.BussinessLogic
{
    public class StatusConfigHandler: IStatusConfigHandler
    {
        private readonly RepositoryHandler<StatusConfig, StatusConfigModel, StatusConfigQueryModel> _statusConfigHandler
               = new RepositoryHandler<StatusConfig, StatusConfigModel, StatusConfigQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<StatusConfigHandler> _logger;

        public StatusConfigHandler(ILogger<StatusConfigHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }    
        public async Task<Response> GetAllActive()
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_STATUS_CONFIG.GET_ALL_ACTIVE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);               

                return await _statusConfigHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
