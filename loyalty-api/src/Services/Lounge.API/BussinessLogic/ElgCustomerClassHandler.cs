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
    public class ElgCustomerClassHandler : IElgCustomerClassHandler
    {
        private readonly RepositoryHandler<ElgCustomerClass, ElgCustomerClassBaseModel, ElgCustomerClassQueryModel> _elgConfigurateHandler
               = new RepositoryHandler<ElgCustomerClass, ElgCustomerClassBaseModel, ElgCustomerClassQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgCustomerClassHandler> _logger;

        public ElgCustomerClassHandler(ILogger<ElgCustomerClassHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> CreateAsync(ElgCustomerClassCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
               

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_CLASS.PRC_CREATE_CUSTOMER_CLASS_V2");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_orderview", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgConfigurateHandler.ExecuteProcOracle(procName, dyParam);


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

        public async Task<Response> UpdateAsync(decimal id, ElgCustomerClassCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_CLASS.PRC_UPDATE_CUSTOMER_CLASS_V2");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_class_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_orderview", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_description", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgConfigurateHandler.ExecuteProcOracle(procName, dyParam);


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

        public async Task<Response> GetByFilterAsync(ElgCustomerClassQueryModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_CLASS.PRC_FILTER_CUSTOMER_CLASS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_status_id", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgConfigurateHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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

        public async Task<Response> UpdateStatusAsync(decimal id, string status, ELoungeBaseModel baseModel)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_CLASS.PRC_UPDATE_CUST_CLASS_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_class_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgConfigurateHandler.ExecuteProcOracle(procName, dyParam);

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

        public async Task<Response> GetAllByConditionAsync(string condition)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_CLASS.PRC_GET_ALL_CUSTOMER_CLASS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", condition, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgConfigurateHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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
