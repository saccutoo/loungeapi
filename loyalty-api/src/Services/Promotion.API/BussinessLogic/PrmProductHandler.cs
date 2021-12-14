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
    public class PrmProductHandler : IPrmProductHandler
    {
        private readonly RepositoryHandler<PrmProduct, PrmProductModel, PrmProductQueryModel> _prmProductHandler
               = new RepositoryHandler<PrmProduct, PrmProductModel, PrmProductQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<PrmProductHandler> _logger;

        public PrmProductHandler(ILogger<PrmProductHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        public async Task<Response> GetByFilterAsync(PrmProductQueryModel queryModel)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT.GET_BY_FILTER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PAGE_SIZE", queryModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_PAGE_INDEX", queryModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_FULL_TEXT_SEARCH", queryModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STATUS", queryModel.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CHANNEL", queryModel.Channel, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _prmProductHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT.GET_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);                

                return await _prmProductHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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
        public async Task<Response> CreateAsync(PrmProductCreateModel model)
        {

            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT.CREATE_RECORD");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_CODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_NAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CHANNEL", model.Channel, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CUSTOMERTYPE", model.CustomerType, OracleMappingType.Varchar2, ParameterDirection.Input);               
                dyParam.Add("P_ORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_CREATEDBY", model.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmProductHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result.StatusCode == StatusCode.Fail)
                {
                    result.Data.Message = "Lỗi ngoại lệ";
                    if (result.Data.Name.Equals("EXISTED")) result.Data.Message = "Không được thêm trùng sản phẩm";
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
        public async Task<Response> UpdateAsync(decimal id, PrmProductUpdateModel model)
        {

            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT.UPDATE_RECORD");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_CODE", model.Code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_NAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CHANNEL", model.Channel, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CUSTOMERTYPE", model.CustomerType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ORDERVIEW", model.OrderView, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_LASTMODIFIEDBY", model.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmProductHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT.DELETE_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);               

                var result = await _prmProductHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result.StatusCode == StatusCode.Fail)
                {
                    result.Data.Message = "Lỗi ngoại lệ";
                    if (result.Data.Name.Equals("USED")) result.Data.Message = "Sản phẩm đang được sử dụng";
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
    }
}
