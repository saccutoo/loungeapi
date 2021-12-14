using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using Utils;
using API.Models;
using API.Infrastructure.Migrations;
using System.Collections.Generic;

namespace API.BussinessLogic
{
    public class PrmOtherConditionHandler
    {
        private readonly RepositoryHandler<PrmOtherCondition, PrmOtherConditionModel, PrmOtherConditionQueryModel> _prmOtherConditionHandler
               = new RepositoryHandler<PrmOtherCondition, PrmOtherConditionModel, PrmOtherConditionQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<PrmOtherConditionHandler> _logger;

        public PrmOtherConditionHandler(ILogger<PrmOtherConditionHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        
        public async Task<Response> GetByProductionInstanceIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_OTHER_CONDITION.GET_BY_PRODUCTION_INSTANCE_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PRODUCTION_INSTANCE_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _prmOtherConditionHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> CreateMultiAsync(IDbConnection iConn, IDbTransaction iTrans, decimal prodInsId, List<PrmOtherConditionCreateModel> listModel)
        {

            try
            {
                if (listModel != null && listModel.Count > 0)
                {
                    foreach (var condition in listModel)
                    {
                        var procName = string.Join('.', _dBSchemaName, "PKG_PRM_OTHER_CONDITION.CREATE_RECORD");
                        var dyParam = new OracleDynamicParameters();
                        dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam.Add("P_PRODUCTIONINSTANCEID", prodInsId, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_CRITERIA", condition.Criteria, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_CONDITION", condition.Condition, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_VALUE", condition.Value, OracleMappingType.Varchar2, ParameterDirection.Input);                        

                        var result = await _prmOtherConditionHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                    }
                    var creatResult = new Response(StatusCode.Success, "");
                    return creatResult;
                }
                return new ResponseError(StatusCode.Fail, "Dữ liệu đầu vào trống");
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
        public async Task<Response> DeleteByProductionInstanceIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_OTHER_CONDITION.DELETE_BY_PROD_INSTANCE_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PRODUCTIONINSTANCEID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _prmOtherConditionHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
