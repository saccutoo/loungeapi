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
using System.Net;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Lounge.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.BussinessLogic
{
    public class ElgVoucherMappingHandler : IElgVoucherMappingHandler
    {

        private readonly RepositoryHandler<ElgUplTransaction, ElgUplTransaction, ElgCustomerQueryModel> _elgUploadIdHandler
               = new RepositoryHandler<ElgUplTransaction, ElgUplTransaction, ElgCustomerQueryModel>();

        private readonly RepositoryHandler<ElgVoucherMappingModel, ElgVoucherMappingBaseModel, ElgVoucherMappingQueryModel> _elgVoucherMappingHandler
               = new RepositoryHandler<ElgVoucherMappingModel, ElgVoucherMappingBaseModel, ElgVoucherMappingQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<ElgVoucherMappingHandler> _logger;
        private readonly RepositoryHandler<ElgVoucherMappingModel, ElgVoucherMappingBaseModel, ElgVoucherMappingQueryModel> _elgVoucherPosHandler
               = new RepositoryHandler<ElgVoucherMappingModel, ElgVoucherMappingBaseModel, ElgVoucherMappingQueryModel>();
        private readonly RepositoryHandler<VucMapVoucherCust, VucMapVoucherCust, ElgVoucherMappingQueryModel> _vucPublishedVoucherTypeHandler
               = new RepositoryHandler<VucMapVoucherCust, VucMapVoucherCust, ElgVoucherMappingQueryModel>();

        private readonly RepositoryHandler<ElgCustInCoreModel, ElgCustInCoreModel, ElgVoucherMappingQueryModel> _elgCustInCoreHandler
               = new RepositoryHandler<ElgCustInCoreModel, ElgCustInCoreModel, ElgVoucherMappingQueryModel>();


        private readonly RepositoryHandler<ElgVoucherSendMailModel, ElgVoucherSendMailModel, ElgVoucherMappingQueryModel> _elgVoucherSendMailHandler
               = new RepositoryHandler<ElgVoucherSendMailModel, ElgVoucherSendMailModel, ElgVoucherMappingQueryModel>();

        private const int MAX_TOTAL_CIF_SEND_DB = 100;

        public ElgVoucherMappingHandler(ILogger<ElgVoucherMappingHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        #region GET DATA

        public async Task<Response> GetApprovedUploadIds(string templateIds, int pageSize, int pageIndex)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.GET_UPLOAD_FILTER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", pageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", pageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", "", OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_list_template_id", templateIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_application_code", "", OracleMappingType.Varchar2, ParameterDirection.Input);


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

        public async Task<Response> GetFileUploads(string pos)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.GET_LIST_UPLOADID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_pos", pos, OracleMappingType.Varchar2, ParameterDirection.Input);
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

        public async Task<Response> GetByFilterAsync(ElgVoucherMappingQueryModel model)
        {
            try
            {
                _logger.LogInformation("ElgVoucherPos - GetByFilterAsync - REQ: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.GET_BY_FILTER");
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

                var result = await _elgVoucherMappingHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                
                _logger.LogInformation("ElgVoucherPos - GetByFilterAsync - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetCustByCif(string cif)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.GET_CUST_BY_CIF");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cif", cif, OracleMappingType.Varchar2, ParameterDirection.Input);
                var result = await _elgCustInCoreHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);

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

        #endregion GET DATA

        #region CRUD DATA

        public async Task<Response> MappingVoucherCustomer(VucMappingVoucherCustModel model, EVoucherBaseModel baseModel)
        {
            try
            {
                if (model.ListCustomer == null || model.ListCustomer.Count < 1)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                _logger.LogInformation("MapVoucherCustomerEbank - REQ: model: " + JsonConvert.SerializeObject(model) + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                // Tinh tong so mang customer can goi
                int nListCustomer = (int)(model.ListCustomer.Count / MAX_TOTAL_CIF_SEND_DB);
                if (model.ListCustomer.Count % MAX_TOTAL_CIF_SEND_DB < MAX_TOTAL_CIF_SEND_DB)
                    nListCustomer += 1;

                // Prepare new list customer data
                List<string> arrCustData = new List<string>();

                // Get List customer to temp list
                List<CustVoucherModel> listCust = model.ListCustomer;
                for (int i = 0; i < nListCustomer; i++)
                {
                    string custData = string.Empty;
                    int indexLeft = listCust.Count > MAX_TOTAL_CIF_SEND_DB ? MAX_TOTAL_CIF_SEND_DB : listCust.Count;
                    for (int j = 0; j < indexLeft; j++)
                    {
                        var cust = listCust[j];
                        custData += cust.CustomerId + "*" + cust.CustomerName + "*" + cust.MaxUsedPerCust + "*" + cust.Pos + "*" + cust.VouCustomerId + "_" ;
                        if (j == indexLeft - 1)
                        {
                            listCust.RemoveRange(0, indexLeft);
                            break;
                        }
                    }
                    arrCustData.Add(custData);
                }

                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();
                    _logger.LogInformation("MapVoucherCustomerEbank - arrCustData: " + JsonConvert.SerializeObject(arrCustData));
                    for (int i = 0; i < arrCustData.Count; i++)
                    {
                        try
                        {
                            var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.MAPPING_VUC_CUSTOMER");
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("i_voucher_id", model.VoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("i_trans_type", model.TransType, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_upload_id", model.UploadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_listCust", arrCustData[i], OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_modify_by", baseModel.LastModifyBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                            var result = await _vucPublishedVoucherTypeHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                            _logger.LogInformation("MapVoucherCustomerEbank - result.Data: " + JsonConvert.SerializeObject(result.Data));
                            if (result.Data != null && !result.Data.Status.Equals("00"))
                            {
                                iTrans.Rollback();
                                return new ResponseObject<ResponseModel>(result.Data, "Không thành công", StatusCode.Fail);
                            }
                        }
                        catch (Exception ex)
                        {
                            iTrans.Rollback();
                            if (_logger != null)
                            {
                                _logger.LogError(ex, "Exception Error");
                                return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                            }
                            else throw ex;
                        }
                    }
                    iTrans.Commit();
                    return new ResponseObject<ResponseModel>(null, "Thành công", StatusCode.Success);
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

    
        public async Task<Response> GetMapVoucherListByFilter(VucMapVoucherCustQueryModel model)
        {
            try
            {
                _logger.LogInformation("GetMapVoucherListByFilter - REQ: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.PRC_FILTER_VOUCHER_MAPPING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_text_search", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_type", model.TransType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_id", model.VoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_pos", model.Pos, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_channel_id", model.ChannelId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_issue_batch_id", model.IssueBatchId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_voucher_type_id", model.VoucherTypeId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _vucPublishedVoucherTypeHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetMapVoucherListByFilter - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> Approved(string listIds, string userName, string pos, decimal uploadId)
        {
            try
            {
                var classId = Helpers.GetConfig("ELG_VOUCHER_CUST:CLASSID");
                var custTypeId = Helpers.GetConfig("ELG_VOUCHER_CUST:CUSTTYPEID");
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.APPROVED_VUC_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("i_listid", listIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_approved_by", userName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_pos", pos, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_uploadtransactionid", uploadId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_class_id", classId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", custTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var result = await _elgVoucherPosHandler.ExecuteProcOracle(procName, dyParam);
                if(result.StatusCode == StatusCode.Success)
                {
                    SendMailVoucher(listIds, userName, pos, uploadId);
                }
                _logger.LogInformation("VoucherMapping - APPROVED_VUC_CUSTOMER - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> SendMailVoucher(string listIds, string userName, string pos, decimal uploadId)
        {
            try
            {
                var classId = Helpers.GetConfig("ELG_VOUCHER_CUST:CLASSID");
                var custTypeId = Helpers.GetConfig("ELG_VOUCHER_CUST:CUSTTYPEID");
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.GET_CUSTOMER_VOUCHER_BY_LISTID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("i_listid", listIds, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_approved_by", userName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_pos", pos, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_uploadtransactionid", uploadId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_class_id", classId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_type_id", custTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);

                var result = await _elgVoucherSendMailHandler.ExecuteProcOracleReturnRow(procName, dyParam);
                _logger.LogInformation("PKG_ELG_CMS_CUSTOMER_VOUCHER.GET_CUSTOMER_VOUCHER_BY_LISTID - RES:" + JsonConvert.SerializeObject(result));
                if (result.StatusCode == StatusCode.Success)
                {
                    var listVoucherEmail = (ResponseObject<List<ElgVoucherSendMailModel>>)result;
                    var dataSendMail = listVoucherEmail.Data;
                    if(dataSendMail.Count > 0)
                    {
                        EmailHandler emailHandler = new EmailHandler();
                        foreach(var item in dataSendMail)
                        {
                            if (string.IsNullOrEmpty(item.Email))
                            {
                                continue;
                            }
                            var exDate = item.ExpireDate.ToString("dd/MM/yyyy");
                            //if (item.ExpireDate > item.VucExpiredDate)
                            //{
                            //    exDate = item.VucExpiredDate.ToString("dd/MM/yyyy");
                            //}
                            
                            await emailHandler.SendMailSuccessfullyVoucher(item.Email, item.FullName, item.VucSerial, exDate);
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

        public async Task<Response> CreateAsync(VucMappingManualVoucherCustModel createModel, EVoucherBaseModel baseModel)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CUSTOMER_VOUCHER.INSERT_ELG_CUSTOMER_VOUCHER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_custid", createModel.CustomerId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_fullname", createModel.CustomerName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_classid", createModel.ClassId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_custtype", createModel.CustTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_expired", createModel.ExpiredDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_posid", createModel.Pos, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_posname", createModel.PosName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cifnum", createModel.CIF, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_createby", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_email", createModel.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phonenum", createModel.PhoneNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_refnum", createModel.RefNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_programname", createModel.ProgramName, OracleMappingType.Varchar2, ParameterDirection.Input);
                _logger.LogDebug("PKG_ELG_CMS_CUSTOMER_VOUCHER.INSERT_ELG_CUSTOMER_VOUCHER " + JsonConvert.SerializeObject(dyParam));
                var result = await _elgVoucherPosHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("INSERT_ELG_CUSTOMER_VOUCHER RES:" + JsonConvert.SerializeObject(result));

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
        #endregion CRUD DATA
    }
}
