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
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class ElgCustomerHandler : IElgCustomerHandler
    {
        private readonly RepositoryHandler<ElgUploadId, ElgUploadId, ElgCustomerQueryModel> _elgUploadIdHandler
               = new RepositoryHandler<ElgUploadId, ElgUploadId, ElgCustomerQueryModel>();

        private readonly RepositoryHandler<ElgCustomer, ElgCustomerBaseModel, ElgCustomerQueryModel> _elgCustomerHandler
               = new RepositoryHandler<ElgCustomer, ElgCustomerBaseModel, ElgCustomerQueryModel>();

        private readonly RepositoryHandler<ElgKYCCustomer, ElgKYCCustomer, ElgCustomerQueryModel> _elgKYCCustomerHandler
               = new RepositoryHandler<ElgKYCCustomer, ElgKYCCustomer, ElgCustomerQueryModel>();

        private const string STATUS_CANCEL = "CANCEL";
        private const string STATUS_UNUSED = "UNUSED";

        private string _dBSchemaName;
        private readonly ILogger<ElgCustomerHandler> _logger;

        public ElgCustomerHandler(ILogger<ElgCustomerHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> CreateAsync(ElgCustomerCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {              
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_CREATE_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_name ", model.PosName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_upload_id", model.UploadId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cif_num", model.CifNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_fullname", model.FullName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_represent_name", model.RepresentUserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_represent_id", model.RepresentUserId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_birthday", model.BirthDay, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_email", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phone_num", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_position", model.Position, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_gender", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (result != null && result.StatusCode == StatusCode.Fail && result.Data != null)
                    return new ResponseError(StatusCode.Fail, result.Data.Name);
                else
                    return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ Thêm khách hàng không thành công!");
                }
                else throw ex;
            }
        }

        public async Task<Response> UpdateAsync(decimal id, ElgCustomerCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_UPDATE_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_customer_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_name ", model.PosName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_upload_id", model.UploadId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cif_num", model.CifNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_fullname", model.FullName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_represent_name", model.RepresentUserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_represent_id", model.RepresentUserId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_birthday", model.BirthDay, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_email", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phone_num", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_gender", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result != null && result.StatusCode == StatusCode.Fail && result.Data != null)
                    return new ResponseError(StatusCode.Fail, result.Data.Name);
                else
                    return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ Sửa khách hàng không thành công!");
                }
                else throw ex;
            }
        }

        public async Task<Response> GetByFilterAsync(ElgCustomerQueryModel model)
        {
            try
            {
                
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_FILTER_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_upload_id", model.UPLOADTRANSACTIONID, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                //dyParam.Add("i_cif_num", model.CifNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                //dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_status_id", model.Status, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

             

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
                

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_UPDATE_CUSTOMER_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_customer_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam);

                
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
               

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_ALL_CUSTOMERS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", condition, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

               

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

        public async Task<Response> GetByIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_CUSTOMER_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_customer_id", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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

        public async Task<Response> KYCCustomerAsync(string query)
        {
            try
            {
                // search customer first
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_SEARCH_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var respCustomerKYC = await _elgKYCCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgKYCCustomer>>;

                if (_logger != null)
                    _logger.LogInformation("ElgCustomer - KYCCustomerAsync - RES:" + JsonConvert.SerializeObject(respCustomerKYC));

                if (respCustomerKYC == null || respCustomerKYC.Data == null)
                {
                    if (_logger != null)
                        _logger.LogInformation("ElgCustomer - KYCCustomerAsync - Check CustomerKYC: respCustomerKYC == null");
                    return null;
                }
                else
                {
                    string id = string.Empty;
                    decimal[] voucherid = new decimal[respCustomerKYC.Data.Count];
                    decimal[] maxusedquantitypercust = new decimal[respCustomerKYC.Data.Count];
                    decimal[] countused = new decimal[respCustomerKYC.Data.Count];
                    string[] vouchername = new string[respCustomerKYC.Data.Count];
                    var typeCheck = "CIF";
                    for(int i = 0; i < respCustomerKYC.Data.Count; i++)
                    {
                        ElgKYCCustomer elgKYCCustomer = respCustomerKYC.Data[i];
                        if (elgKYCCustomer.CustId != null && elgKYCCustomer.CustId.Length > 0)
                        {
                            typeCheck = "CIF";
                            id = elgKYCCustomer.CustId;
                            if(STATUS_UNUSED.Equals(elgKYCCustomer.Cus_Voucher_Status))
                            {
                                voucherid[i] = elgKYCCustomer.CustVoucherId;
                                vouchername[i] = elgKYCCustomer.CustVoucherName;
                                maxusedquantitypercust[i] = elgKYCCustomer.Cus_MaxUsedQuantity;
                                countused[i] = elgKYCCustomer.Cus_CountUsed;
                            }                            
                        }
                        else if (elgKYCCustomer.CustomerId != null && elgKYCCustomer.CustomerId.Length > 0)
                        {
                            typeCheck = "VOUCHER";
                            id = elgKYCCustomer.CustomerId;
                            voucherid[i] = elgKYCCustomer.MapVoucherId;
                            vouchername[i] = elgKYCCustomer.VoucherName;
                            maxusedquantitypercust[i] = elgKYCCustomer.Vuc_MaxUsedQuantity;
                            countused[i] = elgKYCCustomer.Vuc_CountUsed;
                        }
                        else
                            return null;
                    }
                    
                    //lay thong tin khach hang
                    procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_KYC_CUSTOMERS");
                    dyParam = new OracleDynamicParameters();
                    dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("i_text_search", id, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("i_type_check", typeCheck, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgCustomerBaseModel>> ;

                    if (result?.Data != null)
                    {
                        foreach(var item in result.Data)
                        {
                            item.MapVoucherId = voucherid;
                            item.VoucherName = vouchername;
                            item.MaxUsedQuantityPerCust = maxusedquantitypercust;
                            item.CountUsed = countused;
                        }
                        //result.Data.MapVoucherId = voucherid;
                        //result.Data.VoucherName = vouchername;
                        //result.Data.MaxUsedQuantityPerCust = maxusedquantitypercust;
                        //result.Data.CountUsed = countused;
                    }

                    return result;
                }

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

        public async Task<Response> ExportCustomerAsync(ElgCustomerExportModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_EXPORT_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_upload_id", model.UploadId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos_id", model.PosId, OracleMappingType.Decimal, ParameterDirection.Input);
                //dyParam.Add("i_cif_num", model.CifNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", model.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_class_id", model.CustClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                //dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_status_id", model.StatusId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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

        public async Task<Response> UpdateUploadIDStatusAsync(decimal uploadid, string new_status, ELoungeBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("ElgCustomer - UpdateUploadIDStatusAsync - REQ: id: " + uploadid);
                var procName = string.Empty;
                var dyParam = new OracleDynamicParameters();
                if ("APPROVED".Equals(new_status))
                {
                    procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_EXPIRED_CUST_BY_UPLOAD_ID");                    
                    dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("i_upload_id", uploadid, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                    _logger.LogInformation("ElgCustomer - UpdateUploadIDStatusAsync - RES:" + JsonConvert.SerializeObject(result));
                    if (result != null && result.StatusCode == StatusCode.Fail && result.Data != null)
                    {
                        _logger.LogInformation("ElgCustomer - PRC_EXPIRED_CUST_BY_UPLOAD_ID failed");
                        return new ResponseError(StatusCode.Fail, result.Data.Name);
                    }
                }

                procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_UPDATE_CUST_BY_UPLOAD_ID");
                dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_upload_id", uploadid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", new_status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var resultUpdate = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (resultUpdate != null && resultUpdate.StatusCode == StatusCode.Fail && resultUpdate.Data != null)
                {
                    _logger.LogInformation("ElgCustomer - PRC_APPROVE_CUST_BY_UPLOAD_ID failed");
                    return new ResponseError(StatusCode.Fail, resultUpdate.Data.Name);
                }
                else
                    return resultUpdate;
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

        public async Task<Response> GetApprovedUploadIds()
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_APPROVED_UPLOADIDS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var result = await _elgUploadIdHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
              
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

        public async Task<Response> GetPendingUploadIds()
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_PENDING_UPLOADIDS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var result = await _elgUploadIdHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("ElgCustomer - GetPendingUploadIds - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> UpdateMultiUploadIDStatusAsync(decimal old_uploadid, decimal new_uploadid, string status, ELoungeBaseModel baseModel)
        {
            try
            {
                var procName = string.Empty;

                // Update status for newest upload id
                procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_UPDATE_CUST_BY_UPLOAD_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_upload_id", old_uploadid, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var resultUpdate = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (resultUpdate != null && resultUpdate.StatusCode == StatusCode.Fail && resultUpdate.Data != null)
                {
                    _logger.LogInformation("ElgCustomer - PRC_UPDATE_CUST_BY_UPLOAD_ID failed");
                    return new ResponseError(StatusCode.Fail, resultUpdate.Data.Name);
                }


                if ("WAITING_APPROVE".Equals(status))
                {
                    // Update newst upload id to previous upload id
                    // Upload id 1: previous upload id
                    // Upload id 2: newest upload id
                    procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_UPDATE_CUST_LOG");
                    dyParam = new OracleDynamicParameters();
                    dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("i_old_uploadid", old_uploadid, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("i_new_uploadid", new_uploadid, OracleMappingType.Decimal, ParameterDirection.Input);

                    var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                    _logger.LogInformation("ElgCustomer - UpdateMultiUploadIDStatusAsync - RES:" + JsonConvert.SerializeObject(result));
                    if (result != null && result.StatusCode == StatusCode.Fail && result.Data != null)
                    {
                        _logger.LogInformation("ElgCustomer - PRC_UPDATE_CUST_LOG failed");
                        return new ResponseError(StatusCode.Fail, result.Data.Name);
                    }
                }
                
                return resultUpdate;
                
                //using (var unitOfWorkOracle = new UnitOfWorkOracle())
                //{
                //    var iConn = unitOfWorkOracle.GetConnection();
                //    var iTrans = iConn.BeginTransaction();

                //    for (int i = 0; i < elgCustomers.Count; i++)
                //    {
                //        try
                //        {
                //            ElgCustomerApproveModel model = elgCustomers[i];
                //            // Call store on oracle
                //            var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_UPDATE_CUSTOMER_STATUS");
                //            var dyParam = new OracleDynamicParameters();
                //            dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                //            dyParam.Add("i_customer_id", model.CustId, OracleMappingType.Decimal, ParameterDirection.Input);
                //            dyParam.Add("i_new_status", "APPROVED", OracleMappingType.Varchar2, ParameterDirection.Input);
                //            dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                //            var result = await _elgCustomerHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;

                //            _logger.LogInformation("ApproveUploadIDAsync - RES:" + JsonConvert.SerializeObject(result));

                //            if (result.Data != null && !result.Data.Status.Equals("00"))
                //            {
                //                iTrans.Rollback();
                //                return new ResponseObject<ResponseModel>(result.Data, "Không thành công", StatusCode.Fail);
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            iTrans.Rollback();
                //            if (_logger != null)
                //            {
                //                _logger.LogError(ex, "Exception Error");
                //                return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                //            }
                //            else throw ex;
                //        }
                //    }
                //    iTrans.Commit();
                //    return new ResponseObject<ResponseModel>(null, "Thành công", StatusCode.Success);
                //}
                
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

        public async Task<Response> FilterByUploadIds(decimal uploadid1, decimal uploadid2)
        {
            try
            {
               
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_FILTER_BY_UPLOADIDS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_upload_id_1", uploadid1, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_upload_id_2", uploadid2, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("ElgCustomer - GetByFilterAsync - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> ResolveConflictAsync(decimal id, ElgCustomerResolveConflictModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_RESOLVE_CONFLICT_CUST");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_customer_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cif_num", model.CifNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_email", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phone_num", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (result != null && result.StatusCode == StatusCode.Fail && result.Data != null)
                    return new ResponseError(StatusCode.Fail, result.Data.Name);
                else
                    return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ Sửa khách hàng không thành công!");
                }
                else throw ex;
            }
        }

        public async Task<Response> KYCCustomerV2Async(string query, decimal elgCustId)
        {
            try
            {
                //lay thong tin khach hang
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_SEARCH_CUSTOMER_V2");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", query, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_elg_cust_id", elgCustId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgCustomerBaseModel>>;

                if (result?.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        item.MapVoucherId = item.ItemMapVoucherId > 0 ? new decimal[] { item.ItemMapVoucherId } : new decimal[0];
                        item.VoucherName = string.IsNullOrEmpty(item.ItemVoucherName) ? new string[0] : new string[] { item.ItemVoucherName }; ;
                        item.MaxUsedQuantityPerCust = item.ItemMaxUsedQuantityPercust > 0 ? new decimal[] { item.ItemMaxUsedQuantityPercust } : new decimal[0]; ;
                        item.CountUsed = item.ItemCountUsed > 0 ? new decimal[] { item.ItemCountUsed } : new decimal[0]; ; ;
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

        //VINHTQ1
        public async Task<Response> UpdateDetailCustomer(ElgCustomerBaseModel model)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_EDIT_DETAIL_ELG_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_id", model.Id, OracleMappingType.Int32, ParameterDirection.Input);
                dyParam.Add("i_gender", model.Gender, OracleMappingType.NVarchar2, ParameterDirection.Input);
                dyParam.Add("i_birthday ", model.BirthDay, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_position", model.Position, OracleMappingType.NVarchar2, ParameterDirection.Input);
                dyParam.Add("i_cusid", model.CustId, OracleMappingType.NVarchar2, ParameterDirection.Input);
                dyParam.Add("i_phonenum", model.PhoneNum, OracleMappingType.NVarchar2, ParameterDirection.Input);
                dyParam.Add("i_maxpeoplegowith", model.MaxPeopleGoWith, OracleMappingType.NVarchar2, ParameterDirection.Input);
                dyParam.Add("i_type_update", model.TypeUpdate, OracleMappingType.NVarchar2, ParameterDirection.Input);
                dyParam.Add("i_updateby", model.CreateBy, OracleMappingType.NVarchar2, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result != null && result.StatusCode == StatusCode.Fail && result.Data != null)
                    return new ResponseError(StatusCode.Fail, result.Data.Name);
                else
                    return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ Sửa khách hàng không thành công!");
                }
                else throw ex;
            }
        }


        public async Task<Response> GetElgCustomerById(decimal id)
        {
            try
            {
                //lay thong tin khach hang
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_ELG_CUSTOMER_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_id", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgCustomerBaseModel>>;

                if (result?.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        item.MapVoucherId = item.ItemMapVoucherId > 0 ? new decimal[] { item.ItemMapVoucherId } : new decimal[0];
                        item.VoucherName = string.IsNullOrEmpty(item.ItemVoucherName) ? new string[0] : new string[] { item.ItemVoucherName }; ;
                        item.MaxUsedQuantityPerCust = item.ItemMaxUsedQuantityPercust > 0 ? new decimal[] { item.ItemMaxUsedQuantityPercust } : new decimal[0]; ;
                        item.CountUsed = item.ItemCountUsed > 0 ? new decimal[] { item.ItemCountUsed } : new decimal[0]; ; ;
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

        public async Task<Response> GetByCustIdAsync(string elgCustId)
        {
            try
            {
                //lay thong tin khach hang
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER.PRC_GET_CUSTOMER_BY_CUSTID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_custid", elgCustId, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgCustomerBaseModel>>;             

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


        //vinhtq::07/12/2021
        //hàm search danh sách khách hàng mới.trả ra list customer

        public async Task<Response> GetListCustomerV3Async(string textSearch,string fullName,string phoneNum,string cusname,string representUserName,string email)
        {
            try
            {
                ResponseObject<ElgFaceCustomerViewModel> responseFaceCustomer = null;
                if (!string.IsNullOrEmpty(textSearch))
                {
                    ElgFaceCustomerHandler _elgFaceCustomerHandler = new ElgFaceCustomerHandler();
                    responseFaceCustomer = await _elgFaceCustomerHandler.GetByFaceIdAsync(textSearch) as ResponseObject<ElgFaceCustomerViewModel>;
                    if (responseFaceCustomer==null || responseFaceCustomer.Data==null)
                    {
                        return new ResponseError(StatusCode.Fail, "Khách hàng này chưa được liên kết thông tin PCSB với hình ảnh cá nhân. Vui lòng tìm kiếm khách hàng và checkin để tạo liên kết !!!");
                    }
                    cusname = responseFaceCustomer.Data.CustId;
                    if (string.IsNullOrEmpty(cusname))
                    {
                        phoneNum = responseFaceCustomer.Data.PhoneNum;
                    }
                }
                //Lấy thông tin danh sách khách hàng tìm kiếm ra được
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CUSTOMER_NEW.PRC_SEARCH_CUSTOMER_V3");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", string.IsNullOrEmpty(textSearch) ? null : textSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_full_name", string.IsNullOrEmpty(fullName) ? null : fullName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phonenum", string.IsNullOrEmpty(phoneNum) ? null : phoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cusname", string.IsNullOrEmpty(cusname) ? null : cusname, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_representusername", string.IsNullOrEmpty(representUserName) ? null : representUserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_email", string.IsNullOrEmpty(email) ? null : email, OracleMappingType.Varchar2, ParameterDirection.Input);
                var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgCustomerBaseModel>>;
                if (result != null && result.Data != null && result.Data.Count > 0 && !string.IsNullOrEmpty(textSearch))
                {
                    //List<ElgCustomerBaseModel> dataNew = new List<ElgCustomerBaseModel>();
                    //dataNew.Add(result.Data.FirstOrDefault());
                    //result.Data = dataNew;
                    return result;
                }
                else 
                {
                    procName = string.Join('.', _dBSchemaName, "PKG_ELG_CUSTOMER_NEW.PRC_SEARCH_CUSTOMER_EXPIRE");
                    string stringListCustId = string.Empty;
                    if (result != null && result.Data != null && result.Data.Count > 0)
                    {
                        var listCustId = result.Data.Select(x => x.CustId).Distinct();
                        stringListCustId = string.Join(",", listCustId);
                    }
                    dyParam.Add("i_list_custid", stringListCustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                    var resultExpire = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<ElgCustomerBaseModel>>;
                    if (resultExpire != null && resultExpire.Data != null && resultExpire.Data.Count > 0)
                    {
                        if (result !=null && result.Data!=null)
                        {
                            result.Data.AddRange(resultExpire.Data);
                        }
                        else
                        {
                            result = new ResponseObject<List<ElgCustomerBaseModel>>(null,"",StatusCode.Success);
                            result.Data = new List<ElgCustomerBaseModel>();
                            result.Data.AddRange(resultExpire.Data);

                        }
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

        public async Task<Response> GetDistinctByCustIdAsync(string custId)
        {
            //Lấy thông tin danh sách khách hàng tìm kiếm ra được
            var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CUSTOMER_NEW.PRC_DISTINCT_BY_CUSTID");
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
            dyParam.Add("i_cust_id", string.IsNullOrEmpty(custId) ? null : custId, OracleMappingType.Varchar2, ParameterDirection.Input);
            var result = await _elgCustomerHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
            return result;
        }
    }
}
