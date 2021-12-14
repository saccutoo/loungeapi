using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using Utils;
using API.Interface;
using API.Models;
using API.Infrastructure.Migrations;

namespace API.BussinessLogic
{
    public class CriteriaConditionHandler : ICriteriaConditionHandler
    {
        private readonly RepositoryHandler<Criteria, CriteriaModel, CriteriaQueryModel> _criteriaHandler
               = new RepositoryHandler<Criteria, CriteriaModel, CriteriaQueryModel>();

        private readonly RepositoryHandler<Condition, ConditionModel, ConditionQueryModel> _conditionHandler
               = new RepositoryHandler<Condition, ConditionModel, ConditionQueryModel>(); 
        private readonly string _dBSchemaName;
        private readonly ILogger<CriteriaConditionHandler> _logger;

        public CriteriaConditionHandler(ILogger<CriteriaConditionHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        public async Task<Response> GetAllCriteriaActive()
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_CRITERIA.GET_ALL_ACTIVE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);                

                return await _criteriaHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetConditionByCriteria(string criteriaCode)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_CRITERIA.GET_CONDITION_BY_CRITERIA");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_CRITRIA_CODE", criteriaCode, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _conditionHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
