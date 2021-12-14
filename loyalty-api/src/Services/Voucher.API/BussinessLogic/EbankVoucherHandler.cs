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
    public class EbankVoucherHandler : IEbankVoucherHandler
    {
        private readonly RepositoryHandler<EbankVoucher, EbankVoucherBaseModel, EbankVoucherQueryModel> _ebankVoucherHandler
               = new RepositoryHandler<EbankVoucher, EbankVoucherBaseModel, EbankVoucherQueryModel>();

        private string _dBSchemaName;
        private readonly ILogger<EbankVoucherHandler> _logger;
        public EbankVoucherHandler(ILogger<EbankVoucherHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> GetVoucherList(string customerId, string channelId, string tranType, decimal tranAmount)
        {
            try
            {
                _logger.LogInformation("CIF:" + customerId + "|GetVoucherList - REQ: channelId: " + channelId + "|trantype: " + tranType + "|tranamount: " + tranAmount);

                var procName = string.Join('.', "PKG_VUC_VOUCHER_API.PRC_API_GET_VOUCHER_EBANK");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_voucher_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cif_num", customerId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_channel", channelId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_type", tranType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_amount", tranAmount, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _ebankVoucherHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("CIF:" + customerId + "|GetVoucherList - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> UpdateElgVoucherSync(EbankVoucherUpdateModel model)
        {
            try
            {
                _logger.LogInformation("CIF:" + model.CustomerId + "|UpdateElgVoucherSync - REQ:" + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', "PKG_VUC_VOUCHER_API.PRC_API_EBANK_UPDATE_VOUCHER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_pin_num", model.PinNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cif_num", model.CustomerId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_channel", model.ChannelId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_type", model.TranType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_amount", model.TranAmount, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_discount_amount", model.DiscountAmount, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_trans_refno", model.TranRefNo, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _ebankVoucherHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("CIF:" + model.CustomerId + "|UpdateElgVoucherSync - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> CheckElgVoucherSync(EbankVoucherCheckModel model)
        {
            try
            {
                _logger.LogInformation("CIF:" +model.CustomerId+ "|CheckElgVoucherSync - REQ:" + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', "PKG_VUC_VOUCHER_API.PRC_API_EBANK_CHECK_VOUCHER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cif_num", model.CustomerId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_channel", model.ChannelId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_type", model.TranType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_amount", model.TranAmount, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_discount_amount", model.DiscountAmount, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pin_num", model.PinNum, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _ebankVoucherHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("CIF:" + model.CustomerId + "|CheckElgVoucherSync - RES:" + JsonConvert.SerializeObject(result));

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
