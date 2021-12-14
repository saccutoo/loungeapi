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
    public class ElgCustomerTypeHandler : IElgCustomerTypeHandler
    {
        private readonly RepositoryHandler<ElgCustomerType, ElgCustomerTypeBaseModel, ElgCustomerTypeQueryModel> _elgCustomerTypeHandler
               = new RepositoryHandler<ElgCustomerType, ElgCustomerTypeBaseModel, ElgCustomerTypeQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgCustomerTypeHandler> _logger;

        public ElgCustomerTypeHandler(ILogger<ElgCustomerTypeHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetAllByConditionAsync(string condition)
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_LIST_CUSTOMER_TYPES");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", condition, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerTypeHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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
