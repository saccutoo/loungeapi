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
    public class VucVoucherAmtConditionsHandler : IVucVoucherAmtConditionsHandler
    {
        private readonly RepositoryHandler<VucVoucherAmtConditions, VucVoucherAmtConditionsBaseModel, VucVoucherAmtConditionsQueryModel> _vucVoucherAmtConditionHandler
               = new RepositoryHandler<VucVoucherAmtConditions, VucVoucherAmtConditionsBaseModel, VucVoucherAmtConditionsQueryModel>();

        private string _dBSchemaName;
        private readonly ILogger<VucVoucherAmtConditionsHandler> _logger;
        public VucVoucherAmtConditionsHandler(ILogger<VucVoucherAmtConditionsHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> CreateAsync(VucVoucherAmtConditionsCreateModel model, EVoucherBaseModel baseModel)
        {
            try
            {
                if(_logger != null)
                    _logger.LogInformation("VucVoucherAmtConditionsHandler CreateAsync - REQ: model: " + JsonConvert.SerializeObject(model) + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_ADD_VOUCHER_CONDITION");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", model.VoucherId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_amount_valuation", model.AmountValuation, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_percent_valuation", model.PercentValuation, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_min_trans_amount", model.MinTransAmount, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_max_trans_amount", model.MaxTransAmount, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_max_amount_coupon", model.MaxAmountCoupon, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucVoucherAmtConditionHandler.ExecuteProcOracle(procName, dyParam);

                if (_logger != null)
                    _logger.LogInformation("VucVoucherAmtConditionsHandler CreateAsync - RES: " + JsonConvert.SerializeObject(result));

                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                throw ex;
            }
        }

        public async Task<Response> GetVoucherConditionsAll(decimal voucherId)
        {
            try
            {
                if (_logger != null)
                    _logger.LogInformation("GetVoucherConditionsAll - REQ: voucherId" + voucherId);

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_GET_VOUCHER_CONDITIONS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", voucherId, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucVoucherAmtConditionHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                if (_logger != null)
                    _logger.LogInformation("GetVoucherConditionsAll - RES:" + JsonConvert.SerializeObject(result));

                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                throw ex;
            }
        }

        public async Task<Response> DeleteAsync(decimal voucherId)
        {
            try
            {
                if (_logger != null)
                    _logger.LogInformation("VucVoucherAmtConditionsHandler DeleteAsync - REQ: voucherId" + voucherId);

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_REMOVE_VOUCHER_CONDITIONS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", voucherId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _vucVoucherAmtConditionHandler.ExecuteProcOracle(procName, dyParam);

                if (_logger != null)
                    _logger.LogInformation("VucVoucherAmtConditionsHandler DeleteAsync - RES:" + JsonConvert.SerializeObject(result));

                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                }
                throw ex;
            }
        }
    }
}
