using API.Infrastructure.Repositories;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utils;
using Voucher.API.Infrastructure.Migrations;
using Voucher.API.Interface;
using Voucher.API.Models;

namespace Voucher.API.BussinessLogic
{
    public class VucMapVoucherCustHandler : IVucMapVoucherCust
    {
        private readonly RepositoryHandler<VucMapVoucherCust, VucMapVoucherCustBaseModel, VucMapVoucherCustQueryModel> _vucPublishedVoucherTypeHandler
               = new RepositoryHandler<VucMapVoucherCust, VucMapVoucherCustBaseModel, VucMapVoucherCustQueryModel>();
        private string _dBSchemaName;
        private readonly ILogger<VucMapVoucherCustHandler> _logger;
        private const int MAX_TOTAL_CIF_SEND_DB = 100;

        public VucMapVoucherCustHandler(ILogger<VucMapVoucherCustHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> ValidateVoucherCustomer(decimal voucherId, string transType, decimal numOfVoucherTarget)
        {
            try
            {
                _logger.LogInformation("ValidateVoucherCustomer - REQ: voucherId: " + voucherId + "|transType: " + transType + "|numOfVoucherTarget: " + numOfVoucherTarget);

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_MAP_VOUCHER_CUST.PRC_VALIDATE_VOUCHER_CUSTOMER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", voucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_trans_type", transType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_num_of_voucher_target", numOfVoucherTarget, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _vucPublishedVoucherTypeHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("ValidateVoucherCustomer - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetMapVoucherListByFilter(VucMapVoucherCustQueryModel model)
        {
            try
            {
                _logger.LogInformation("GetMapVoucherListByFilter - REQ: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_MAP_VOUCHER_CUST.PRC_FILTER_VOUCHER_MAPPING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_text_search", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_trans_type", model.TransType, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_id", model.VoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
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

        public async Task<Response> CancelVoucherMapping(decimal mapId, EVoucherBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("CancelVoucherMapping - REQ: mapId: " + mapId + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_MAP_VOUCHER_CUST.PRC_CANCEL_VOUCHER_MAPPING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_map_id", mapId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_modify_by", baseModel.LastModifyBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucPublishedVoucherTypeHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("CancelVoucherMapping - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> MapVoucherCustomerEbank(VucMapVoucherCustMappingModel model, EVoucherBaseModel baseModel)
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
                List<UsedPerCustModel> listCust = model.ListCustomer;
                for (int i = 0; i < nListCustomer; i++)
                {
                    string custData = string.Empty;
                    int indexLeft = listCust.Count > MAX_TOTAL_CIF_SEND_DB ? MAX_TOTAL_CIF_SEND_DB : listCust.Count;
                    for (int j = 0; j < indexLeft; j++)
                    {
                        var cust = listCust[j];
                        custData += cust.CustomerId + "*" + cust.CustomerName + "*" + cust.MaxUsedPerCust + "_";
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

                    for (int i = 0; i < arrCustData.Count; i++)
                    {
                        try
                        {
                            var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_MAP_VOUCHER_CUST.PRC_MAP_VUC_CUSTOMER_EBANK_NEW");
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("i_voucher_id", model.VoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("i_trans_type", model.TransType, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_upload_id", model.UploadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_listCust", arrCustData[i], OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_modify_by", baseModel.LastModifyBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                            var result = await _vucPublishedVoucherTypeHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;

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

        public async Task<Response> MapVoucherCustomerEbankTestCommit(VucMapVoucherCustMappingModel model, EVoucherBaseModel baseModel)
        {
            try
            {
                if (model.ListCustomer == null || model.ListCustomer.Count < 1)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                _logger.LogInformation("MapVoucherCustomerEbank - REQ: model: " + JsonConvert.SerializeObject(model) + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();

                    foreach (var cust in model.ListCustomer)
                    {
                        try
                        {
                            var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_MAP_VOUCHER_CUST.PRC_MAP_VOUCHER_CUSTOMER_EBANK");
                            var dyParam = new OracleDynamicParameters();
                            dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam.Add("i_voucher_id", model.VoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("i_trans_type", model.TransType, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_upload_id", model.UploadId, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_customer_id", cust.CustomerId, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_customer_name", cust.CustomerName, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam.Add("i_max_used_per_cust", cust.MaxUsedPerCust, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam.Add("i_modify_by", baseModel.LastModifyBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                            var result = await _vucPublishedVoucherTypeHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;

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
                }

                return new ResponseObject<ResponseModel>(null, "Thành công", StatusCode.Success);
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
