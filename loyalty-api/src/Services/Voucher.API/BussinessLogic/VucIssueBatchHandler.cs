using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Voucher.API.Interface;
using Voucher.API.Models;
using Voucher.API.Infrastructure.Migrations;
using Newtonsoft.Json;

namespace Voucher.API.BussinessLogic
{
    public class VucIssueBatchHandler : IVucIssueBatchHandler
    {
        private readonly RepositoryHandler<VucIssueBatch, VucIssueBatchBaseModel, VucIssueBatchQueryModel> _vucIssueBatchHandler
               = new RepositoryHandler<VucIssueBatch, VucIssueBatchBaseModel, VucIssueBatchQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<VucIssueBatchHandler> _logger;

        public VucIssueBatchHandler(ILogger<VucIssueBatchHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> CreateAsync(VucIssueBatchCreateUpdateModel model, EVoucherBaseModel baseModel)
        {

            try
            {
                _logger.LogInformation("CreateAsync - REQ:" + JsonConvert.SerializeObject(model) + "|BASEMODEL: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', "PKG_VUC_CMS_ISSUE_BATCH.PRC_CREATE_ISSUE_BATCH");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_issue_date", model.IssueDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucIssueBatchHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("CreateAsync - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetIssueBathById(string id)
        {
            try
            {
                _logger.LogInformation("GetIssueBathById - REQ: id: " + id);

                var procName = string.Join('.', "PKG_VUC_CMS_ISSUE_BATCH.PRC_GET_ISSUE_BATCH_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_issue_batch_id", id, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucIssueBatchHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);

                _logger.LogInformation("GetIssueBathById - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetListByFilter(VucVoucherQueryModel model)
        {
            try
            {
                _logger.LogInformation("GetListByFilter - REQ: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_ISSUE_BATCH.PRC_FILTER_ISSUE_BATCH");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_issue_batch_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucIssueBatchHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListByFilter - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> UpdateAsync(string id, VucIssueBatchCreateUpdateModel model, EVoucherBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("UpdateAsync - REQ:" + "Id: " + id + "|" + JsonConvert.SerializeObject(model) + "|BASEMODEL: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', "PKG_VUC_CMS_ISSUE_BATCH.PRC_UPDATE_ISSUE_BATCH");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_issue_batch_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_issue_date", model.IssueDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_modify_by", baseModel.LastModifyBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucIssueBatchHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("UpdateAsync - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetListIssueBath(string query)
        {
            try
            {
                _logger.LogInformation("GetListIssueBath - REQ: query" + query);

                var procName = string.Join('.', "PKG_VUC_CMS_ISSUE_BATCH.PRC_GET_ALL_ISS_BATCHES");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucIssueBatchHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListIssueBath - RES:" + JsonConvert.SerializeObject(result));

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
