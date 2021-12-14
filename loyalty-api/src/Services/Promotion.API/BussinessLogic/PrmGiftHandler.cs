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
using System.Collections.Generic;

namespace API.BussinessLogic
{
    public class PrmGiftHandler
    {
        private readonly RepositoryHandler<PrmGift, PrmGiftModel, PrmGiftQueryModel> _prmGiftHandler
               = new RepositoryHandler<PrmGift, PrmGiftModel, PrmGiftQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<PrmGiftHandler> _logger;

        public PrmGiftHandler(ILogger<PrmGiftHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }    
        public async Task<Response> GetByProductionInstanceIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_GIFT.GET_BY_PRODUCTION_INSTANCE_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PRODUCTION_INSTANCE_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _prmGiftHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> CreateMultiAsync(IDbConnection iConn, IDbTransaction iTrans, decimal prodInsId, List<PrmGiftCreateModel> listModel)
        {

            try
            {
                if (listModel != null && listModel.Count > 0)
                {
                    foreach (var condition in listModel)
                    {
                        var procName = string.Join('.', _dBSchemaName, "PKG_PRM_GIFT.CREATE_RECORD");
                        var dyParam = new OracleDynamicParameters();
                        dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam.Add("P_NAME", condition.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_PRODUCTIONINSTANCEID", prodInsId, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_IASCODEREFERENCE", condition.IasCodeReference, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_PRICE", condition.Price, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_QUANTITY", condition.Quantity, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_COSTPRICE", condition.CostPrice, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_ACCOUNTNAME", condition.AccountName, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_ACCOUNTNUM", condition.AccountNum, OracleMappingType.Varchar2, ParameterDirection.Input);

                        var result = await _prmGiftHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
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
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_GIFT.DELETE_BY_PROD_INSTANCE_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PRODUCTIONINSTANCEID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _prmGiftHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
