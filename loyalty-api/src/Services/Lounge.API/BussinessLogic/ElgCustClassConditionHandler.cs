using API.Infrastructure.Migrations;
using API.Infrastructure.Repositories;
using API.Interface;
using API.Models;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class ElgCustClassConditionHandler : IElgCustClassConditionHandler
    {
        private readonly RepositoryHandler<ElgCustClassConfition, ElgCustClassConditionBaseModel, ElgCustClassConditionQueryModel> _elgCustClassConfitionHandler
               = new RepositoryHandler<ElgCustClassConfition, ElgCustClassConditionBaseModel, ElgCustClassConditionQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgCustClassConditionHandler> _logger;

        public ElgCustClassConditionHandler(ILogger<ElgCustClassConditionHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> CreateAsync(ElgCustClassConditionCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CLASS_CONDITION.PRC_CREATE_CLASS_CONDITION");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_class_id", model.CustClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_max_checkin", model.MaxCheckin, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_max_people_go_with", model.MaxPeopleGoWith, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_condition_use", model.ConditionUse, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_unlimit_time", model.UnlimitExpireTime, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustClassConfitionHandler.ExecuteProcOracle(procName, dyParam);



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

        public async Task<Response> UpdateAsync(decimal id, ElgCustClassConditionCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
               

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CLASS_CONDITION.PRC_UPDATE_CLASS_CONDITION");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_class_condition_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_class_id", model.CustClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_max_checkin", model.MaxCheckin, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_max_people_go_with", model.MaxPeopleGoWith, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_condition_use", model.ConditionUse, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_unlimit_time", model.UnlimitExpireTime, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustClassConfitionHandler.ExecuteProcOracle(procName, dyParam);

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

        public async Task<Response> GetByFilterAsync(ElgCustClassConditionQueryModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CLASS_CONDITION.PRC_FILTER_CLASS_CONDITION");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_class_id", model.CustClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_max_people_go_with", model.MaxPeopleGoWith, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_max_of_use", model.MaxOfUse, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustClassConfitionHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CLASS_CONDITION.PRC_UPDATE_CLASS_COND_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_class_condition_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustClassConfitionHandler.ExecuteProcOracle(procName, dyParam);

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

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CLASS_CONDITION.PRC_GET_ALL_CLASS_CONDITION");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", condition, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustClassConfitionHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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
