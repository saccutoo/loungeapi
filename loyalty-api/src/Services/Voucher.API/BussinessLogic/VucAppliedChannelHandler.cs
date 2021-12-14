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
    public class VucAppliedChannelHandler : IVucAppliedChannelHandler
    {
        private readonly RepositoryHandler<VucAppliedChannel, VucAppliedChannelBaseModel, VucAppliedChannelQueryModel> _tblVucVoucherTypeHandler
               = new RepositoryHandler<VucAppliedChannel, VucAppliedChannelBaseModel, VucAppliedChannelQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<VucAppliedChannelHandler> _logger;

        public VucAppliedChannelHandler(ILogger<VucAppliedChannelHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetListVoucherChannel(string query)
        {
            try
            {
                _logger.LogInformation("GetListVoucherChannel - REQ: " + query);

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_VOUCHER.PRC_GET_LIST_VOUCHER_CHANNELS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _tblVucVoucherTypeHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListVoucherChannel - RES:" + JsonConvert.SerializeObject(result));

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
