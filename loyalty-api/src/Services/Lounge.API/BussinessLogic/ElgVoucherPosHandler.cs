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
using System.Net;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Lounge.API.Models;

namespace API.BussinessLogic
{
    public class ElgVoucherPosHandler : IElgVoucherPosHandler
    {
        private readonly RepositoryHandler<ElgVoucherPos, ElgVoucherPosModel, ElgVoucherPosQueryModel> _elgVoucherPosHandler
               = new RepositoryHandler<ElgVoucherPos, ElgVoucherPosModel, ElgVoucherPosQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<ElgVoucherPosHandler> _logger;
        private string _apiGatewayRootUrl;
        private string _clientId;
        private string _clientSecret;

        public ElgVoucherPosHandler(ILogger<ElgVoucherPosHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        #region GET DATA
        public async Task<Response> GetByFilterAsync(ElgVoucherPosQueryModel model)
        {
            try
            {
                _logger.LogInformation("ElgVoucherPos - GetByFilterAsync - REQ: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_VOUCHER_POS.GET_BY_FILTER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_pos", model.Pos, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_vuc_code", model.Vuc_Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_status", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgVoucherPosHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("ElgVoucherPos - GetByFilterAsync - RES:" + JsonConvert.SerializeObject(result));

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
        #endregion GET DATA

        #region CRUD DATA
        public async Task<Response> UpdateStatusAsync(ElgVoucherPosChangeStausModel stausModel)
        {
            try
            {
                var t = JsonConvert.SerializeObject(stausModel);
                _logger.LogInformation("VoucherPos - UpdateStatusAsync - REQ: |status: " + t);
                var listId = "";
                if (stausModel.Ids != null)
                {
                    listId = string.Join(",", stausModel.Ids);
                }
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_VOUCHER_POS.UPDATE_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_list_id", listId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_upload_id", stausModel.UploadTransactionId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", stausModel.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                if (stausModel.Status == ElgVoucherPosStausConst.WAITING_APPROVE)
                {
                    dyParam.Add("i_create_by", stausModel.UserAction, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("i_approved_by", "", OracleMappingType.Varchar2, ParameterDirection.Input);
                }
                if (stausModel.Status == ElgVoucherPosStausConst.REJECTED || stausModel.Status == ElgVoucherPosStausConst.APPROVED)
                {
                    dyParam.Add("i_create_by", "", OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("i_approved_by", stausModel.UserAction, OracleMappingType.Varchar2, ParameterDirection.Input);
                }
                
                var result = await _elgVoucherPosHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("VoucherPos - UpdateStatusAsync - RES:" + JsonConvert.SerializeObject(result));

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


        public async Task<Response> CreateAsync(ElgVoucherPosCreateModel createModel)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_VOUCHER_POS.CREATE_VOUCHER_POS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_pos", createModel.Pos, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_vuc_code", createModel.Vuc_Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_quantity", createModel.Quantity, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", createModel.CreateByUser, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_upload_id", createModel.UploadTransactionId, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgVoucherPosHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("VoucherPos - CREATE_VOUCHER_POS - RES:" + JsonConvert.SerializeObject(result));

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
        #endregion CRUD DATA
    }
}
