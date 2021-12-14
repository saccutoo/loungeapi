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
    public class VucVoucherHandler : IVucVoucherHandler
    {
        private readonly RepositoryHandler<VucVoucher, VucVoucherBaseModel, VucVoucherQueryModel> _vucVoucherHandler
               = new RepositoryHandler<VucVoucher, VucVoucherBaseModel, VucVoucherQueryModel>();

        private string _dBSchemaName;
        private readonly ILogger<VucVoucherHandler> _logger;
        private VucVoucherAmtConditionsHandler _vucVoucherAmtConditionHandler;
        private const int MAX_TOTAL_DATA_SEND_DB = 50;

        public VucVoucherHandler(ILogger<VucVoucherHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }

        public async Task<Response> CreateAsync(VucVoucherCreateUpdateModel model, EVoucherBaseModel baseModel)
        {
            _vucVoucherAmtConditionHandler = new VucVoucherAmtConditionsHandler();

            try
            {
                if (model.ListCondition == null || model.ListCondition.Count < 1)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                _logger.LogInformation("VucVoucherHandler CreateAsync - REQ: model: " + JsonConvert.SerializeObject(model) + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_CREATE_VOUCHER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description_vn", model.DescriptionVn, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description_en", model.DescriptionEn, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_issue_batch_id", model.IssueBatchId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_channel_id", model.ChannelId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_voucher_type_id", model.VoucherTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_effective_date", model.EffectiveDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_max_used_quantity", model.MaxUsedQuantity, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_issuequantity", model.IssueQuantity, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_voucher_theme", null, OracleMappingType.Clob, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                _logger.LogInformation("VucVoucherHandler CreateAsync - RES: " + JsonConvert.SerializeObject(result));


                if (result != null && result.Data.Status.Equals("00"))
                {
                    _logger.LogInformation("VucVoucherHandler CreateAsync - BEGIN INSERT VOUCHER CONDITION");

                    foreach (VucVoucherAmtConditionsCreateModel item in model.ListCondition)
                    {
                        item.VoucherId = result.Data.Id;

                        var response = await _vucVoucherAmtConditionHandler.CreateAsync(item, baseModel);
                    }

                    _logger.LogInformation("VucVoucherHandler CreateAsync - END INSERT VOUCHER CONDITION");
                }

                return new ResponseObject<ResponseModel>(result.Data, "Thành công", StatusCode.Success);
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

        public async Task<Response> UpdateAsync(decimal id, VucVoucherCreateUpdateModel model, EVoucherBaseModel baseModel)
        {
            _vucVoucherAmtConditionHandler = new VucVoucherAmtConditionsHandler();

            try
            {
                if (model.ListCondition == null || model.ListCondition.Count < 1)
                {
                    return new ResponseError(StatusCode.Fail, "Không thành công");
                }

                _logger.LogInformation("VucVoucherHandler UpdateAsync - REQ: voucherId: " + id + "|model: " + JsonConvert.SerializeObject(model) + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_UPDATE_VOUCHER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", id, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_name", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description_vn", model.DescriptionVn, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_description_en", model.DescriptionEn, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_batch_id", model.IssueBatchId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_channel_id", model.ChannelId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_effective_date", model.EffectiveDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_expire_date", model.ExpireDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_voucher_type_id", model.VoucherTypeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_max_used_quantity", model.MaxUsedQuantity, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_issuequantity", model.IssueQuantity, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_voucher_theme", null, OracleMappingType.Clob, ParameterDirection.Input);
                dyParam.Add("i_modify_by", baseModel.LastModifyBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                _logger.LogInformation("VucVoucherHandler VucVoucherHandler - RES: " + JsonConvert.SerializeObject(result));

                if (result != null && result.Data.Status.Equals("00"))
                {
                    _logger.LogInformation("VucVoucherHandler CreateAsync - BEGIN DELETE ALL VOUCHER CONDITION BY VOUCHER ID : " + id);

                    var deleteCondition = await _vucVoucherAmtConditionHandler.DeleteAsync(id) as ResponseObject<ResponseModel>;

                    _logger.LogInformation("VucVoucherHandler CreateAsync - END DELETE ALL VOUCHER CONDITION BY VOUCHER ID : " + id);

                    if (deleteCondition != null && deleteCondition.Data.Status.Equals("00"))
                    {
                        _logger.LogInformation("VucVoucherHandler CreateAsync - BEGIN INSERT VOUCHER CONDITION");

                        foreach (VucVoucherAmtConditionsCreateModel item in model.ListCondition)
                        {
                            item.VoucherId = id;

                            var response = await _vucVoucherAmtConditionHandler.CreateAsync(item, baseModel);
                        }

                        _logger.LogInformation("VucVoucherHandler CreateAsync - END INSERT VOUCHER CONDITION");
                    }

                }

                return new ResponseObject<ResponseModel>(result.Data, "Thành công", StatusCode.Success);

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

        public async Task<Response> GetListVoucherByFilter(VucVoucherQueryModel model)
        {
            try
            {
                _logger.LogInformation("GetListVoucherByFilter - REQ: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_VOUCHER.PRC_FILTER_VOUCHERS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_voucher_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_search_iss_batch_id", model.IssueBatchId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_voucher_type_id", model.VoucherType, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_channel_id", model.ChannelId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_status_id", model.StatusId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListVoucherByFilter - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetListVoucherAll(string query)
        {
            try
            {
                _logger.LogInformation("GetListVoucherAll - REQ: query" + query);

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_GET_ALL_VOUCHERS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListVoucherAll - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetVoucherByID(decimal voucherId)
        {
            try
            {
                _logger.LogInformation("GetVoucherDetail - REQ: voucherId" + voucherId);

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_GET_VOUCHER_DETAILS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", voucherId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);

                _logger.LogInformation("GetVoucherDetail - RES:" + JsonConvert.SerializeObject(result));

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


        public async Task<Response> ApproveVoucher(decimal voucherId, EVoucherBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("ApproveVoucher - REQ: voucherId" + voucherId + "|baseModel: " + JsonConvert.SerializeObject(baseModel));
                // Get voucher by ID
                var voucherDetail = await GetVoucherByID(voucherId) as ResponseObject<VucVoucherBaseModel>;
                if (voucherDetail != null && voucherDetail.StatusCode == StatusCode.Success)
                {
                    _logger.LogInformation("VucVoucherHandler GetVoucherByID Successfully");
                    var status = voucherDetail.Data.Status;
                    var issuequantity = voucherDetail.Data.IssueQuantity;
                    if (status.Equals("WAITING_APPROVE"))
                    {
                        // Gen issuequantity number of GUID
                        List<string> lstGUIDs = new List<string>();
                        for (int i = 0; i < issuequantity; i++)
                        {
                            string guid = System.Guid.NewGuid().ToString().ToUpper().Replace("-","");
                            lstGUIDs.Add(guid);
                        }
                        // Tinh tong so mang Data can goi
                        int nListData = (int)(issuequantity / MAX_TOTAL_DATA_SEND_DB);
                        if (issuequantity % MAX_TOTAL_DATA_SEND_DB < MAX_TOTAL_DATA_SEND_DB)
                            nListData += 1;

                        // Prepare new list GUI data
                        List<string> arrGuidData = new List<string>();

                        // Get List customer to temp list
                        for (int i = 0; i < nListData; i++)
                        {
                            string Data = string.Empty;
                            int indexLeft = lstGUIDs.Count > MAX_TOTAL_DATA_SEND_DB ? MAX_TOTAL_DATA_SEND_DB : lstGUIDs.Count;
                            for (int j = 0; j < indexLeft; j++)
                            {
                                var guid = lstGUIDs[j];
                                Data += guid + "_";
                                if (j == indexLeft - 1)
                                {
                                    lstGUIDs.RemoveRange(0, indexLeft);
                                    break;
                                }
                            }
                            arrGuidData.Add(Data);
                        }

                        using (var unitOfWorkOracle = new UnitOfWorkOracle())
                        {
                            var iConn = unitOfWorkOracle.GetConnection();
                            var iTrans = iConn.BeginTransaction();

                            for (int i = 0; i < arrGuidData.Count; i++)
                            {
                                try
                                {
                                    // Call store on oracle
                                    var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_APPROVE_VOUCHER");
                                    var dyParam = new OracleDynamicParameters();
                                    dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                    dyParam.Add("i_voucher_id", voucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                                    dyParam.Add("i_issue_quantity", issuequantity, OracleMappingType.Decimal, ParameterDirection.Input);
                                    dyParam.Add("i_listPin", arrGuidData[i], OracleMappingType.Varchar2, ParameterDirection.Input);
                                    dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                                    var result = await _vucVoucherHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;

                                    _logger.LogInformation("ApproveVoucher - RES:" + JsonConvert.SerializeObject(result));

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
                    else
                    {
                        return new ResponseError(StatusCode.Fail, "Trạng thái hiện tại không được phép thay đổi");
                    }
                }
                else
                {
                    return new ResponseError(StatusCode.Fail, "Không lấy được voucher detail");
                }
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

        public async Task<Response> RejectVoucher(decimal voucherId, EVoucherBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("RejectVoucher - REQ: voucherId" + voucherId + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_REJECT_VOUCHER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", voucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("RejectVoucher - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> CancelVoucher(decimal voucherId, EVoucherBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("CancelVoucher - REQ: voucherId" + voucherId + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', "PKG_VUC_CMS_VOUCHER.PRC_CANCEL_VOUCHER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_voucher_id", voucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("CancelVoucher - RES:" + JsonConvert.SerializeObject(result));

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

        public async Task<Response> GetListVoucherForMapping(decimal channelId, decimal issueBatchId, decimal voucherTypeId)
        {
            try
            {
                _logger.LogInformation("GetListVoucherForMapping - REQ: channelId: " + channelId + "|issueBatchId: " + issueBatchId + "|voucherTypeId: " + voucherTypeId);

                var procName = string.Join('.', _dBSchemaName, "PKG_VUC_CMS_VOUCHER.PRC_GET_VOUCHERS_FOR_MAPPING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_channel_id", channelId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_issue_batch_id", issueBatchId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_voucher_type_id", voucherTypeId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _vucVoucherHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("GetListVoucherForMapping - RES:" + JsonConvert.SerializeObject(result));

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
