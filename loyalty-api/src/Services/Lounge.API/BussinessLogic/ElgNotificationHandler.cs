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
    public class ElgNotificationHandler : IElgNotificationHandler
    {

        private readonly RepositoryHandler<ElgNotificationModel, ElgNotificationViewModel, ElgNotificationQueryModel> _ElgNotificationHandler
               = new RepositoryHandler<ElgNotificationModel, ElgNotificationViewModel, ElgNotificationQueryModel>();



        private const string STATUS_CANCEL = "CANCEL";
        private const string STATUS_UNUSED = "UNUSED";

        private string _dBSchemaName;
        private string _PackageName;
        private readonly ILogger<ElgNotificationHandler> _logger;

        public ElgNotificationHandler(ILogger<ElgNotificationHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
            _PackageName = "PKG_ELG_NOTIFICATION";
        }

        public async Task<Response> GetAllByTypeAsync(string type)
        {
            try
            {
                //Lấy thông tin danh sách khách hàng tìm kiếm ra được
                var procName = _dBSchemaName + "." + _PackageName + "." + "PRC_GET_BY_TYPE";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PTYPE", type, OracleMappingType.Varchar2, ParameterDirection.Input); 
                var result = await _ElgNotificationHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgNotificationViewModel>>;
                if (result!=null && result.Data.Count>0)
                {
                    foreach (var item in result.Data)
                    {
                        item.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(item.Value, item.FaceId));
                    }
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
                else throw ex;
            }
        }

        public async Task<Response> GetByIdAsync(decimal Id)
        {
            try
            {
                //Lấy thông tin danh sách khách hàng tìm kiếm ra được
                var procName = _dBSchemaName + "." + _PackageName + "." + "PRC_GET_BY_ID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", Id, OracleMappingType.Decimal, ParameterDirection.Input);
                var result= await _ElgNotificationHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<ElgNotificationViewModel>;
                if (result != null && result.Data !=null)
                {
                    result.Data.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(result.Data.Value, result.Data.FaceId));
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
                else throw ex;
            }
        }

        public async Task<Response> GetByFaceIdAsync(string faceId)
        {
            try
            {
                //Lấy thông tin danh sách khách hàng tìm kiếm ra được
                var procName = _dBSchemaName + "." + _PackageName + "." + "PRC_GET_FACEID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFACEID", faceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                var result = await _ElgNotificationHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgNotificationViewModel>>;
                if (result != null && result.Data.Count > 0)
                {
                    foreach (var item in result.Data)
                    {
                        item.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(item.Value, item.FaceId));
                    }
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
                else throw ex;
            }
        }

        public async Task<Response> GetByFilterAsync(ElgNotificationQueryModel query)
        {
            try
            {
                //Lấy thông tin danh sách khách hàng tìm kiếm ra được
                var procName = _dBSchemaName + "." + _PackageName + "." + "GET_BY_FILTER";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("pPageSize", query.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pPageIndex", query.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("pFaceId", query.FaceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("pValue", query.PageSize, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var result = await _ElgNotificationHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgNotificationViewModel>>;
                if (result != null && result.Data.Count > 0)
                {
                    foreach (var item in result.Data)
                    {
                        item.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(item.Value, item.FaceId));
                    }
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
                else throw ex;
            }
        }

        public async Task<Response> CreateAsync(ElgNotificationCreateModel model)
        {
            try
            {

                var procName = _dBSchemaName + "." + _PackageName + "." + "INSERT_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFACEID", model.FaceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PVALUE", model.Value, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCREATEBY", model.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _ElgNotificationHandler.ExecuteProcOracle(procName, dyParam);

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
