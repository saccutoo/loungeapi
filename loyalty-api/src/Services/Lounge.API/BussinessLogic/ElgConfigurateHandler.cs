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
    public class ElgConfigurateHandler : IElgConfigurateHandler
    {
        private readonly RepositoryHandler<ElgConfigurate, ElgConfigurateBaseModel, ElgConfigurateQueryModel> _elgConfigurateHandler
               = new RepositoryHandler<ElgConfigurate, ElgConfigurateBaseModel, ElgConfigurateQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<ElgConfigurateHandler> _logger;

        public ElgConfigurateHandler(ILogger<ElgConfigurateHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> CreateAsync(ElgConfigurateCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CONFIGURATE.PRC_CREATE_CONFIGURATE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_value", model.Value, OracleMappingType.Decimal, ParameterDirection.Input);
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

        public async Task<Response> UpdateAsync(decimal id, ElgConfigurateCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
              

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CONFIGURATE.PRC_UPDATE_CONFIGURATE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_configurate_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_value", model.Value, OracleMappingType.Decimal, ParameterDirection.Input);
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

        public async Task<Response> GetByFilterAsync(ElgConfigurateQueryModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CONFIGURATE.PRC_FILTER_CONFIGURATE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_configurate_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);

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
