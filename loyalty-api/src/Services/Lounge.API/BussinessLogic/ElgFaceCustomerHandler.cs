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
    public class ElgFaceCustomerHandler : IElgFaceCustomerHandler
    {

        private readonly RepositoryHandler<ElgFaceCustomerModel, ElgFaceCustomerViewModel, ElgFaceCustomerQueryModel> _ElgFaceCustomerHandler
               = new RepositoryHandler<ElgFaceCustomerModel, ElgFaceCustomerViewModel, ElgFaceCustomerQueryModel>();



        private const string STATUS_CANCEL = "CANCEL";
        private const string STATUS_UNUSED = "UNUSED";

        private string _dBSchemaName;
        private string _PackageName;
        private readonly ILogger<ElgFaceCustomerHandler> _logger;

        public ElgFaceCustomerHandler(ILogger<ElgFaceCustomerHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
            _PackageName = "PKG_ELG_FACE_CUSTOMER";
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
                var result = await _ElgFaceCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgFaceCustomerViewModel>>;
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
                return await _ElgFaceCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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
                return await _ElgFaceCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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
                //Lấy thông tin danh sách khách hàng tìm kiếm ra được
                var procName = _dBSchemaName + "." + _PackageName + "." + "PRC_GET_BY_CUSTID";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PCUSTID", custId, OracleMappingType.Varchar2, ParameterDirection.Input);
                return await _ElgFaceCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

        public async Task<Response> CreateAsync(ElgFaceCustomerCreateModel model)
        {
            try
            {
                var procName = _dBSchemaName + "." + _PackageName + "." + "INSERT_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFACEID", model.FaceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCUSTID", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPHONENUM", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCREATEBY", model.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                var result = await _ElgFaceCustomerHandler.ExecuteProcOracle(procName, dyParam);
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

        public async Task<Response> UpdateAsync(ElgFaceCustomerUpdateModel model)
        {
            try
            {
                var procName = _dBSchemaName + "." + _PackageName + "." + "UPDATE_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFACEID", model.FaceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCUSTID", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PPHONENUM", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PUPDATEBY", model.UpdatedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                var result = await _ElgFaceCustomerHandler.ExecuteProcOracle(procName, dyParam);
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
