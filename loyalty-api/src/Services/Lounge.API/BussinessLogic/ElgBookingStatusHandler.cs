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
    public class ElgBookingStatusHandler : IElgBookingStatusHandler
    {
        private readonly RepositoryHandler<ElgBookingStatus, ElgBookingStatusBaseModel, ElgBookingStatusQueryModel> _elgBookingStatusHandler
               = new RepositoryHandler<ElgBookingStatus, ElgBookingStatusBaseModel, ElgBookingStatusQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgBookingStatusHandler> _logger;

        public ElgBookingStatusHandler(ILogger<ElgBookingStatusHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetAllByConditionAsync(string condition)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_GET_LIST_BOOKING_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", condition, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingStatusHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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
