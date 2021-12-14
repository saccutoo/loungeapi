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
    public class VucVoucherTypeHandler : IVucVoucherTypeHandler
    {
        private readonly RepositoryHandler<VucVoucherType, VucVoucherTypeBaseModel, VucVoucherTypeQueryModel> _tblVucVoucherTypeHandler
               = new RepositoryHandler<VucVoucherType, VucVoucherTypeBaseModel, VucVoucherTypeQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<VucVoucherTypeHandler> _logger;

        public VucVoucherTypeHandler(ILogger<VucVoucherTypeHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetListVoucherType(string query)
        {
            try
            {
                _logger.LogInformation("GetListVoucherType - REQ: " + query);

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_VOUCHER.PRC_GET_LIST_VOUCHER_TYPE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _tblVucVoucherTypeHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListVoucherType - RES:" + JsonConvert.SerializeObject(result));

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
