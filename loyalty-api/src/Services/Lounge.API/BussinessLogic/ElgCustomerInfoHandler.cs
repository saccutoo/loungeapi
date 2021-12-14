using API.Infrastructure.Migrations;
using API.Infrastructure.Repositories;
using API.Interface;
using API.Models;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class ElgCustomerInfoHandler : IElgCustomerInfoHandler
    {

        private readonly RepositoryHandler<ElgCustomerInfo, ElgCustomerInfoModel, ElgCustomerInfoQueryModel> _elgCustomerInfoHandler
               = new RepositoryHandler<ElgCustomerInfo, ElgCustomerInfoModel, ElgCustomerInfoQueryModel>();

        private string _dBSchemaName;
        private readonly ILogger<ElgCustomerInfoHandler> _logger;

        public ElgCustomerInfoHandler(ILogger<ElgCustomerInfoHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> UpdateAsync(decimal id, ElgCustomerInfoUpdateModel model)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_INFO.PRC_UPDATE_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_customer_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cif_num", model.CifNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_fullname", model.FullName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_birthday", model.BirthDay, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_email", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phone_num", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_gender", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_allow_private", model.AllowPrivateRoom, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_position", model.Position, OracleMappingType.NVarchar2, ParameterDirection.Input);

                var result = await _elgCustomerInfoHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result != null && result.StatusCode == StatusCode.Fail && result.Data != null)
                    return new ResponseError(StatusCode.Fail, result.Data.Name);
                else
                    return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ Sửa khách hàng không thành công!");
                }
                else throw ex;
            }
        }
        public async Task<Response> GetByFilterAsync(ElgCustomerInfoQueryModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_INFO.PRC_FILTER_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerInfoHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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
        public async Task<Response> GetByIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_INFO.PRC_GET_CUSTOMER_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_customer_id", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustomerInfoHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);

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

        public async Task<Response> GetByCustIdAsync(string custId)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_INFO.PRC_GET_BY_CUST_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cust_id", custId, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerInfoHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);

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
