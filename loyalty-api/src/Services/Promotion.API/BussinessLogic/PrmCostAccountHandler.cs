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

namespace API.BussinessLogic
{
    public class PrmCostAccountHandler : IPrmCostAccountHandler
    {
        private readonly RepositoryHandler<PrmCostAccount, PrmCostAccountModel, PrmCostAccountQueryModel> _prmCostAccountHandler
               = new RepositoryHandler<PrmCostAccount, PrmCostAccountModel, PrmCostAccountQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<PrmCostAccountHandler> _logger;

        public PrmCostAccountHandler(ILogger<PrmCostAccountHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        public async Task<Response> GetByFilterAsync(PrmCostAccountQueryModel queryModel)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_COST_ACCOUNT.GET_BY_FILTER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PAGE_SIZE", queryModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_PAGE_INDEX", queryModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_FULL_TEXT_SEARCH", queryModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STATUS", queryModel.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_GIFTFORM", queryModel.GiftForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CUSTOMERTYPE", queryModel.CustomerType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ISREGISTERIAT", queryModel.IsRegisterIAT, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _prmCostAccountHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetByIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_COST_ACCOUNT.GET_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _prmCostAccountHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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
        public async Task<Response> CreateAsync(PrmCostAccountCreateModel model)
        {

            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_COST_ACCOUNT.CREATE_RECORD");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ACCOUNTNUMBER", model.AccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ACCOUNTNAME", model.AccountName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_GIFTFORM", model.GiftForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ISREGISTERIAT", model.IsRegisterIAT, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_CUSTOMERTYPE", model.CustomerType, OracleMappingType.Varchar2, ParameterDirection.Input);               
                dyParam.Add("P_ORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_CREATEDBY", model.CreatedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmCostAccountHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result.StatusCode == StatusCode.Fail)
                {
                    result.Data.Message = "Lỗi ngoại lệ";
                    if (result.Data.Name.Equals("EXISTED")) result.Data.Message = "Tài khoản đã được khái báo";
                }
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
        public async Task<Response> UpdateAsync(decimal id, PrmCostAccountUpdateModel model)
        {

            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_COST_ACCOUNT.UPDATE_RECORD");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_ACCOUNTNUMBER", model.AccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ACCOUNTNAME", model.AccountName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_GIFTFORM", model.GiftForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ISREGISTERIAT", model.IsRegisterIAT, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_CUSTOMERTYPE", model.CustomerType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_LASTMODIFIEDBY", model.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmCostAccountHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
        public async Task<Response> DeleteByIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_COST_ACCOUNT.DELETE_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);               

                var result = await _prmCostAccountHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
