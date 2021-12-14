using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Voucher.API.Interface;
using Voucher.API.Models;
using Voucher.API.Infrastructure.Migrations;
using Newtonsoft.Json;

namespace Voucher.API.BussinessLogic
{
    public class VucTransTypeHandler : IVucTransTypeHandler
    {
        private readonly RepositoryHandler<VucTransType, VucTransTypeBaseModel, VucTransTypeQueryModel> _vucTransTypeHandler
               = new RepositoryHandler<VucTransType, VucTransTypeBaseModel, VucTransTypeQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<VucTransTypeHandler> _logger;

        public VucTransTypeHandler(ILogger<VucTransTypeHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetListTransType(string query)
        {
            try
            {
                _logger.LogInformation("GetListTransType - REQ: " + query);

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_MAP_VOUCHER_CUST.PRC_GET_LIST_TRANS_TYPE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucTransTypeHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListTransType - RES:" + JsonConvert.SerializeObject(result));

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
