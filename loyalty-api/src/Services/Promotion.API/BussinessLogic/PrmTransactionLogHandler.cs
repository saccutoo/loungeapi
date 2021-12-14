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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace API.BussinessLogic
{
    public class PrmTransactionLogHandler : IPrmTransactionLogHandler
    {
        private readonly RepositoryHandler<PrmTransactionLog, PrmTransactionLogModel, PrmTransactionLogQueryModel> _prmTransactionLogHandler
               = new RepositoryHandler<PrmTransactionLog, PrmTransactionLogModel, PrmTransactionLogQueryModel>();

        private readonly RepositoryHandler<PrmTransactionLogDetail, PrmTransactionLogDetailModel, PrmTransactionLogDetailQueryModel> _prmTransactionLogDetailHandler
               = new RepositoryHandler<PrmTransactionLogDetail, PrmTransactionLogDetailModel, PrmTransactionLogDetailQueryModel>();

        private readonly RepositoryHandler<AccountingDetail, AccountingDetailModel, PrmPromotionQueryModel> _accountingDetailHandler
               = new RepositoryHandler<AccountingDetail, AccountingDetailModel, PrmPromotionQueryModel>();

        private readonly RepositoryHandler<PrmFinancialPosting, PrmFinancialPosting, PrmPromotionQueryModel> _financialPostingHandler
               = new RepositoryHandler<PrmFinancialPosting, PrmFinancialPosting, PrmPromotionQueryModel>();

        private readonly string _dBSchemaName;
        private readonly string _mainPosCDFinancialPosting;
        private readonly string _keyMD5;
        private readonly string _urlEsbGwApiEndpoint;
        private readonly string _urlCoreApiEndpoint;
        private readonly string _urlIasApiEndpoint;
        private readonly string _accessToken;
        private readonly string _tKTGDVKH;
        private readonly string _tKTTNCN;
        private readonly decimal _thresholdValueTax;
        private readonly decimal _taxPercent;
        private readonly ILogger<PrmTransactionLogHandler> _logger;

        public PrmTransactionLogHandler(ILogger<PrmTransactionLogHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
            _mainPosCDFinancialPosting = Helpers.GetConfig("PromotionConfig:MainPosCDFinancialPosting");
            _tKTGDVKH = Helpers.GetConfig("PromotionConfig:TKTGDVKH");
            _tKTTNCN = Helpers.GetConfig("PromotionConfig:TKTTNCN");
            _thresholdValueTax = Helpers.IsNumber(Helpers.GetConfig("PromotionConfig:ThresholdValueTax")) ? Convert.ToDecimal(Helpers.GetConfig("PromotionConfig:ThresholdValueTax")) : 0;
            _taxPercent = Helpers.IsNumber(Helpers.GetConfig("PromotionConfig:TaxPercent")) ? Convert.ToDecimal(Helpers.GetConfig("PromotionConfig:TaxPercent")) : 10;
            _keyMD5 = Helpers.GetConfig("PromotionConfig:KeyMD5");
            _urlEsbGwApiEndpoint = Helpers.GetConfig("EndPoint:ESBGW:UrlEndpoint");
            _urlIasApiEndpoint = Helpers.GetConfig("EndPoint:IasApi:UrlEndpoint");
            _urlCoreApiEndpoint = Helpers.GetConfig("EndPoint:CoreApi:UrlEndpoint");
            _accessToken = "";
        }
        public async Task<Response> GetByFilterAsync(PrmTransactionLogQueryModel queryModel)
        {
            try
            {
                var dateFromDefault = new DateTime(1, 1, 1, 0, 0, 1);
                var dateToDefault = new DateTime(9999, 12, 30, 23, 59, 59);
                var createdFrom = queryModel.CreatedFrom.HasValue ? new DateTime(queryModel.CreatedFrom.Value.Year, queryModel.CreatedFrom.Value.Month, queryModel.CreatedFrom.Value.Day, 0, 0, 1) : dateFromDefault;
                var approvedFrom = queryModel.ApprovedFrom.HasValue ? new DateTime(queryModel.ApprovedFrom.Value.Year, queryModel.ApprovedFrom.Value.Month, queryModel.ApprovedFrom.Value.Day, 0, 0, 1) : dateFromDefault;
                var createdTo = queryModel.CreatedTo.HasValue ? new DateTime(queryModel.CreatedTo.Value.Year, queryModel.CreatedTo.Value.Month, queryModel.CreatedTo.Value.Day, 23, 59, 59) : dateToDefault;
                var approvedTo = queryModel.ApprovedTo.HasValue ? new DateTime(queryModel.ApprovedTo.Value.Year, queryModel.ApprovedTo.Value.Month, queryModel.ApprovedTo.Value.Day, 23, 59, 59) : dateToDefault;
                if (queryModel.IsFilterRevoke)
                {
                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_TRANS_REVOKE_BY_FILTER");
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_PAGE_SIZE", queryModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_PAGE_INDEX", queryModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_FULL_TEXT_SEARCH", queryModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_STATUS", queryModel.Status != "ALL" ? queryModel.Status : "", OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_SAVEACCOUNTNUMBER", queryModel.SaveAccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_CIFNUMBER", queryModel.CifNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_USERSTAFF", queryModel.UserStaff, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_APPROVEDBY", queryModel.ApprovedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_CREATEDFROM", createdFrom, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_CREATEDTO", createdTo, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_APPROVEDFROM", approvedFrom, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_APPROVEDTO", approvedTo, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_POS", queryModel.Pos, OracleMappingType.Varchar2, ParameterDirection.Input);

                    return await _prmTransactionLogHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
                }
                else
                {
                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_TRANSACTION_LOG_BY_FILTER");
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_PAGE_SIZE", queryModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_PAGE_INDEX", queryModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_FULL_TEXT_SEARCH", queryModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_STATUS", queryModel.Status != "ALL" ? queryModel.Status : "", OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_SAVEACCOUNTNUMBER", queryModel.SaveAccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_CIFNUMBER", queryModel.CifNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_USERSTAFF", queryModel.UserStaff, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_APPROVEDBY", queryModel.ApprovedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_CREATEDFROM", createdFrom, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_CREATEDTO", createdTo, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_APPROVEDFROM", approvedFrom, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_APPROVEDTO", approvedTo, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_POS", queryModel.IsFilterByPos ? queryModel.Pos : "", OracleMappingType.Varchar2, ParameterDirection.Input);

                    return await _prmTransactionLogHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetListAccountingByTransactionId(decimal transactionId)
        {
            try
            {
                var result = new ResponseObject<List<AccountingModel>>(new List<AccountingModel>(), string.Empty, StatusCode.Success);
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_LIST_ACCOUNTING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_TRANSACTIONID", transactionId, OracleMappingType.Decimal, ParameterDirection.Input);
                _logger.LogInformation("GetListAccountingByTransactionId listAccountingResult before: " + transactionId);
                var listAccountingResult = await _accountingDetailHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<AccountingDetailModel>>;
                _logger.LogInformation("GetListAccountingByTransactionId listAccountingResult: " + JsonConvert.SerializeObject(listAccountingResult));
                if (listAccountingResult != null && listAccountingResult.StatusCode == StatusCode.Success)
                {

                    var listProductCode = listAccountingResult.Data.Select(sp => sp.PromotionCode).Distinct().ToList();
                    var saveAccNumber = listAccountingResult.Data.Where(sp => !string.IsNullOrEmpty(sp.SaveAccountNumber)).First().SaveAccountNumber;
                    var customerName = listAccountingResult.Data.Where(sp => !string.IsNullOrEmpty(sp.CustomerName)).First().CustomerName;

                    foreach (var productCode in listProductCode)
                    {
                        // 1.Nhóm theo CTKM
                        var accountingModel = new AccountingModel
                        {
                            PromotionCode = productCode,
                            PromotionName = listAccountingResult.Data.Where(sp => sp.PromotionCode == productCode).FirstOrDefault().PromotionName,
                        };
                        // 1.1 Nhóm các bút toán giống nhau
                        var listAccountingByPromo = listAccountingResult.Data.Where(sp => sp.PromotionCode == productCode).ToList();
                        if (listAccountingByPromo != null && listAccountingByPromo.Count > 0)
                        {
                            foreach (var accounting in listAccountingByPromo)
                            {
                                // Quà tặng
                                if (string.Compare(accounting.GiftForm, "Quà tặng") == 0)
                                {
                                    accounting.ToAccountName = "Tài khoản chi phí NVL";
                                    //accounting.Amount = accounting.Quantity * accounting.CostPrice;
                                    accounting.Amount = accounting.TotalPayment;
                                }
                                // Chuyển khoản
                                if (string.Compare(accounting.GiftForm, "Chuyển khoản") == 0)
                                {
                                    accounting.ToAccountName = accounting.CustomerName;
                                    accounting.Amount = accounting.GiftValue;
                                }
                                // Tiền mặt
                                if (string.Compare(accounting.GiftForm, "Tiền mặt") == 0)
                                {
                                    accounting.ToAccountName = "TK trung gian ĐVKD";
                                    accounting.Amount = accounting.GiftValue;
                                }
                                accounting.Remarks = "CHI KHUYEN MAI THEO CHUONG TRINH_" + accounting.PromotionName + "_" + accounting.CifNumber;
                                accounting.FinancialPostingRequest = InitFinancialPosting(accounting);
                            }
                        }
                        accountingModel.ListAccountingDetailModel = listAccountingByPromo;

                        result.Data.Add(accountingModel);
                    }
                    _logger.LogInformation("GetListAccountingByTransactionId result.Data:" + JsonConvert.SerializeObject(result) + " - saveAccNumber: " + saveAccNumber);
                    // Kiểm tra giao dịch có cần nộp thuế hay không? Có thì tạo bút toán nộp thuế
                    var procName1 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_DETAIL_BY_LEGACYREFNO");
                    var dyParam1 = new OracleDynamicParameters();
                    dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam1.Add("P_LEGACYREFNO", saveAccNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam1.Add("P_TRANSACTIONID", transactionId, OracleMappingType.Decimal, ParameterDirection.Input);

                    var listDetailByLegacyRefNo = await _prmTransactionLogDetailHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<PrmTransactionLogDetailModel>>;
                    _logger.LogInformation("GetListAccountingByTransactionId listDetailByLegacyRefNo:" + JsonConvert.SerializeObject(listDetailByLegacyRefNo));
                    if (listDetailByLegacyRefNo != null && listDetailByLegacyRefNo.StatusCode == StatusCode.Success)
                    {
                        decimal tienDaDuyet = 0;
                        decimal tienThuHoi = 0;
                        decimal tienDangChoDuyet = 0;
                        decimal amountTax = 0;

                        tienDaDuyet = listDetailByLegacyRefNo.Data.Where(sp => sp.Status == "APPROVED").FirstOrDefault() != null ? listDetailByLegacyRefNo.Data.Where(sp => sp.Status == "APPROVED").Sum(sp => sp.Quantity * sp.GiftValue) : 0;
                        tienThuHoi = listDetailByLegacyRefNo.Data.Where(sp => sp.Status == "REVOKED").FirstOrDefault() != null ? listDetailByLegacyRefNo.Data.Where(sp => sp.Status == "REVOKED").Sum(sp => sp.Quantity * sp.GiftValue) : 0;
                        tienDangChoDuyet = listDetailByLegacyRefNo.Data.Where(sp => sp.Status == "WAITING_APPROVE").FirstOrDefault() != null ? listDetailByLegacyRefNo.Data.Where(sp => sp.Status == "WAITING_APPROVE").Sum(sp => sp.Quantity * sp.GiftValue) : 0;

                        var accPayTax = listAccountingResult.Data.Where(sp => !string.IsNullOrEmpty(sp.AccountPayTax)).FirstOrDefault() != null ? listAccountingResult.Data.Where(sp => !string.IsNullOrEmpty(sp.AccountPayTax)).FirstOrDefault().AccountPayTax : "";
                        if (tienDangChoDuyet + tienDaDuyet - tienThuHoi > _thresholdValueTax && !string.IsNullOrEmpty(accPayTax))
                        {
                            var taxPos = listAccountingResult.Data.First().PosCd;
                            var taxPosCd = listAccountingResult.Data.First().Pos;
                            var cifNum = listAccountingResult.Data.First().CifNumber;
                            if (tienDaDuyet - tienThuHoi < _thresholdValueTax) amountTax = (tienDangChoDuyet + tienDaDuyet - tienThuHoi - _thresholdValueTax) * (_taxPercent / 100);
                            if (tienDaDuyet - tienThuHoi >= _thresholdValueTax) amountTax = tienDangChoDuyet * (_taxPercent / 100);
                            var accountingModel = new AccountingModel
                            {
                                PromotionCode = "VAT",
                                PromotionName = "Thu thuế TNCN"
                            };
                            accountingModel.ListAccountingDetailModel = new List<AccountingDetailModel>();
                            var vatAccounting = new AccountingDetailModel
                            {
                                ToAccountName = "Tài khoản thuế TNCN",
                                ToAccountNumber = _tKTTNCN,
                                FromAccountName = accPayTax == _tKTGDVKH ? "TK trung gian ĐVKD" : customerName,
                                FromAccountNumber = accPayTax,
                                Amount = (int)amountTax,
                                Pos = taxPos,
                                PosCd = taxPosCd,
                                LogDetailId = 0,
                                Remarks = "THU THUE TNCN THEO SO TIET KIEM_" + saveAccNumber + "_" + cifNum,
                                GiftForm = "Thu thuế TNCN"
                            };
                            vatAccounting.FinancialPostingRequest = InitFinancialPosting(vatAccounting);
                            accountingModel.ListAccountingDetailModel.Add(vatAccounting);
                            result.Data.Add(accountingModel);
                        }
                        _logger.LogInformation("GetListAccountingByTransactionId result.Data2:" + JsonConvert.SerializeObject(result));
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
        public async Task<Response> GetByIdAsync(decimal id)
        {
            try
            {
                var result = new ResponseObject<PrmTransactionLogModel>(new PrmTransactionLogModel(), string.Empty, StatusCode.Success);
                // Lấy thông tin transaction log
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_TRANSACTION_LOG_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var getTransactionLogById = await _prmTransactionLogHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<PrmTransactionLogModel>;
                if (getTransactionLogById != null && getTransactionLogById.StatusCode == StatusCode.Success)
                {

                    result.Data = getTransactionLogById.Data;
                    result.Data.TaiKhoanTrungGianDVKH = _tKTGDVKH;
                    // Lấy thông tin transaction log detail
                    var procName1 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_DETAIL_BY_TRANSACTION");
                    var dyParam1 = new OracleDynamicParameters();
                    dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam1.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                    var getDetailByTransactionId = await _prmTransactionLogDetailHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<PrmTransactionLogDetailModel>>;
                    if (getDetailByTransactionId != null && getDetailByTransactionId.StatusCode == StatusCode.Success)
                    {
                        result.Data.TotalGiftValue = getDetailByTransactionId.Data.Sum(sp => sp.Quantity * sp.GiftValue);
                        var listProductInstance = getDetailByTransactionId.Data.Select(sp => sp.ProductInstanceId).GroupBy(sp => sp).ToList();
                        result.Data.ListPrmProductTransactionLogModel = new List<PrmProductTransactionLogModel>();
                        foreach (var procductId in listProductInstance)
                        {
                            var productInstance = getDetailByTransactionId.Data.Where(sp => sp.ProductInstanceId == procductId.Key).FirstOrDefault();
                            var productInstanceModel = new PrmProductTransactionLogModel
                            {
                                ListPrmTransactionLogDetailModel = new List<PrmTransactionLogDetailModel>(),
                                ProductInstanceId = productInstance.ProductInstanceId,
                                PromotionCode = productInstance.PromotionCode,
                                PromotionId = productInstance.PromotionId,
                                PromotionName = productInstance.PromotionName,
                                SpendForm = productInstance.SpendForm,
                                TransactionId = productInstance.TransactionId,
                                GiftCashValue = productInstance.GiftCashValue,

                            };
                            productInstanceModel.ListPrmTransactionLogDetailModel = getDetailByTransactionId.Data.Where(sp => sp.ProductInstanceId == procductId.Key).ToList();

                            result.Data.ListPrmProductTransactionLogModel.Add(productInstanceModel);

                        }
                    }
                    //else return new ResponseObject<PrmTransactionLogModel>(null, "Không có dữ liệu", StatusCode.Fail);
                }
                else return new ResponseObject<PrmTransactionLogModel>(null, "Không có dữ liệu", StatusCode.Fail);
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
        public async Task<Response> CreateLogAsync(PrmTransactionLogCreateModel model)
        {
            try
            {
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();
                    var result = new ResponseObject<ResponseModel>(null, string.Empty, StatusCode.Fail);
                    // Kiểm tra bút toán trên 10M đã gửi thông tin tài khoản nộp thuế chưa?
                    _logger.LogInformation("CreateLogAsync Start" + JsonConvert.SerializeObject(model));
                    if ((model.Status == "IN_DEBT" || model.Status == "REJECTED") && model.Id > 0)
                    {
                        var status = "WAITING_APPROVE";
                        if (model.IsInDebt == 1) status = "IN_DEBT";
                        var procName0 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.UPDATE_TRANSACTION_LOG");
                        var dyParam0 = new OracleDynamicParameters();
                        dyParam0.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam0.Add("P_ID", model.Id, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam0.Add("P_ISINDEBT", model.IsInDebt, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam0.Add("P_STATUS", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam0.Add("P_USERSTAFF", model.UserStaff, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam0.Add("P_ACCOUNTPAYTAX", model.AccountPayTax, OracleMappingType.Varchar2, ParameterDirection.Input);
                        result = await _prmTransactionLogHandler.ExecuteProcOracle(procName0, iConn, iTrans, dyParam0) as ResponseObject<ResponseModel>;
                        _logger.LogInformation("CreateLogAsync PKG_PRM_TRANSACTION_LOG.UPDATE_TRANSACTION_LOG" + JsonConvert.SerializeObject(result));
                    }
                    else
                    {
                        var giftInDebtJson = "";
                        if (model.ListGiftInDebt != null && model.ListGiftInDebt.Count > 0) giftInDebtJson = JsonConvert.SerializeObject(model.ListGiftInDebt);

                        var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CREATE_TRANSACTION_LOG");
                        var dyParam = new OracleDynamicParameters();
                        dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam.Add("P_POS", model.Pos, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_POSDESC", model.PosDesc, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_DEPTID", model.DeptId, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_USERSTAFF", model.UserStaff, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_CIFNUMBER", model.CifNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_CUSTOMERNAME", model.CustomerName, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_ACCOUNTNUMBER", model.AccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_SAVEACCOUNTNUMBER", model.SaveAccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_OPENDATE", model.OpenDate, OracleMappingType.Date, ParameterDirection.Input);
                        dyParam.Add("P_DUEDATE", model.DueDate, OracleMappingType.Date, ParameterDirection.Input);
                        dyParam.Add("P_DEPOSITTERM", model.DepositTerm, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_ISINDEBT", model.IsInDebt, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_DEPOSITVALUE", model.DepositValue, OracleMappingType.Decimal, ParameterDirection.Input);
                        dyParam.Add("P_LICENSE", model.Lincense, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_PHONE", model.Phone, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_GIFTINDEBT", giftInDebtJson, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_ACCOUNTPAYTAX", model.AccountPayTax, OracleMappingType.Varchar2, ParameterDirection.Input);
                        result = await _prmTransactionLogHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                        _logger.LogInformation("CreateLogAsync PKG_PRM_TRANSACTION_LOG.CREATE_TRANSACTION_LOG" + JsonConvert.SerializeObject(result));
                    }

                    if (result.StatusCode == StatusCode.Success)
                    {
                        var transactionId = result.Data.Id;
                        if (model.Status == "REJECTED" || model.Status == "IN_DEBT")
                        {
                            // Nếu giao dịch gửi phê duyệt lại sau từ chối hoặc chuyển sang nợ quà, xóa các log detail cũ
                            var procName2 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.DELETE_DETAIL_BY_TRANSACTION");
                            var dyParam2 = new OracleDynamicParameters();
                            dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam2.Add("P_TRANSACTIONID", transactionId, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam2.Add("P_TYPE", 0, OracleMappingType.Decimal, ParameterDirection.Input);

                            var deleteresult = await _prmTransactionLogHandler.ExecuteProcOracle(procName2, iConn, iTrans, dyParam2) as ResponseObject<ResponseModel>;
                            if (deleteresult == null || deleteresult.StatusCode == StatusCode.Fail) return new ResponseError(StatusCode.Fail, "Không xóa được giao dịch cũ!");
                            _logger.LogInformation("CreateLogAsync PKG_PRM_TRANSACTION_LOG.DELETE_DETAIL_BY_TRANSACTION" + JsonConvert.SerializeObject(deleteresult));
                        }

                        // Thêm Log detail. Giao dịch nợ quà không được thêm chi tiết
                        if (model.ListPrmProductTransactionLogModel != null && model.ListPrmProductTransactionLogModel.Count > 0 && model.IsInDebt != 1)
                        {
                            var listPrmProductTransactionLogValid = model.ListPrmProductTransactionLogModel.Where(sp => sp.ListPrmTransactionLogDetailModel.Count > 0).ToList();
                            if ((listPrmProductTransactionLogValid == null || listPrmProductTransactionLogValid.Count == 0) && model.IsInDebt != 1) return new ResponseError(StatusCode.Fail, "Chưa chọn quà tặng!");
                            foreach (var product in listPrmProductTransactionLogValid)
                            {
                                var procName1 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CREATE_TRANSACTION_LOGDETAIL");
                                foreach (var detail in product.ListPrmTransactionLogDetailModel)
                                {
                                    try
                                    {
                                        var lstGiftStockDetail = new List<IAS_NVLItemDetailModel>();
                                        double totalPayment = 0;
                                        decimal transHeadId = 0;
                                        if (detail.IasCodeReference != "0")
                                        {
                                            // Lấy chi tiết hàng theo kho
                                            using (var client = new HttpClient())
                                            {
                                                string getIasStockOnHandUri = _urlIasApiEndpoint + "api/ias/nvl/get-stock-onhand?listItemId=" + detail.IasCodeReference;
                                                client.DefaultRequestHeaders.Clear();
                                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                                                client.DefaultRequestHeaders.Add("X-PosCd", model.Pos);
                                                client.DefaultRequestHeaders.Add("X-DeptId", model.DeptId);
                                                _logger.LogInformation("CreateLogAsyncget-stock-onhand?listItemId=" + detail.IasCodeReference);
                                                var task = await client.GetAsync(getIasStockOnHandUri);
                                                var jsonString = task.Content.ReadAsStringAsync();
                                                _logger.LogInformation("CreateLogAsync get-stock-onhand : jsonString " + jsonString);
                                                ResponseEndPointList<IAS_NVLItemDetailModel> resGetStockOnHand = JsonConvert.DeserializeObject<ResponseEndPointList<IAS_NVLItemDetailModel>>(jsonString.Result);
                                                if (resGetStockOnHand == null || resGetStockOnHand.StatusCode == StatusCode.Fail || resGetStockOnHand.Data.Count == 0) return new ResponseError(StatusCode.Fail, "Không lấy được thông tin tồn kho của quà tặng!");
                                                else
                                                {
                                                    // Kiểm tra số lượng tồn kho thực tế có đủ với số lượng gửi yêu cầu
                                                    var slTonTongCacLo = resGetStockOnHand.Data.Sum(sp => sp.QTY_TRANS);
                                                    if (slTonTongCacLo < detail.Quantity) return new ResponseError(StatusCode.Fail, "Số lượng quà tồn không đủ với yêu cầu, kiểm tra lại giao dịch 'Quà tặng'!");
                                                }
                                                // Phân bổ hàng hóa lấy theo lô
                                                decimal slCanLay = detail.Quantity;
                                                decimal slDaLay = 0;
                                                decimal slConLai = 0;
                                                var allStockHasItem = resGetStockOnHand.Data.OrderBy(sp => sp.DETAIL_ID_LINK).ToList();
                                                for (int i = 0; i < allStockHasItem.Count; i++)
                                                {
                                                    slConLai = slCanLay - slDaLay;
                                                    var item = allStockHasItem[i];
                                                    if (slConLai <= 0) break;
                                                    if (slConLai <= item.QTY_TRANS)
                                                    {
                                                        slDaLay += slConLai;
                                                        lstGiftStockDetail.Add(new IAS_NVLItemDetailModel
                                                        {
                                                            ITEM_ID = item.ITEM_ID,
                                                            TRANS_DETAIL_ID = 0,
                                                            DETAIL_ID_LINK = item.DETAIL_ID_LINK,
                                                            PRICE_NO_VAT = item.PRICE_NO_VAT,
                                                            QTY_TRANS = slConLai
                                                        });
                                                        break;
                                                    }
                                                    if (slConLai > item.QTY_TRANS)
                                                    {
                                                        slDaLay += item.QTY_TRANS;
                                                        lstGiftStockDetail.Add(new IAS_NVLItemDetailModel
                                                        {
                                                            ITEM_ID = item.ITEM_ID,
                                                            TRANS_DETAIL_ID = 0,
                                                            DETAIL_ID_LINK = item.DETAIL_ID_LINK,
                                                            PRICE_NO_VAT = item.PRICE_NO_VAT,
                                                            QTY_TRANS = item.QTY_TRANS
                                                        });
                                                    }
                                                }
                                                // Tạo phiếu xuất kho và giữ hàng
                                                string makeTransAndHoldUri = _urlIasApiEndpoint + "api/ias/nvl/trans-and-hold";
                                                client.DefaultRequestHeaders.Clear();
                                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                                                client.DefaultRequestHeaders.Add("X-PosCd", model.Pos);
                                                client.DefaultRequestHeaders.Add("X-DeptId", model.DeptId);

                                                var exportTransCreateModel = new IAS_ExportTransCreateModel
                                                {
                                                    DeptCd = model.DeptId,
                                                    PosCd = model.Pos,
                                                    PromoId = product.PromotionId,
                                                    UserName = model.UserStaff,
                                                    ItemRequest = lstGiftStockDetail
                                                };

                                                var jsonQueryModel = JsonConvert.SerializeObject(exportTransCreateModel);
                                                _logger.LogInformation("CreateLogAsync jsonQueryModel:" + jsonQueryModel);
                                                var buffer = Encoding.UTF8.GetBytes(jsonQueryModel);
                                                var byteContent = new ByteArrayContent(buffer);
                                                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                                var task1 = await client.PostAsync(makeTransAndHoldUri, byteContent);
                                                var jsonString1 = task1.Content.ReadAsStringAsync();
                                                _logger.LogInformation("CreateLogAsync jsonString1: " + jsonString1);

                                                ResponseEndPoint<IASTransHeadRes> resMakeTransAndHold = JsonConvert.DeserializeObject<ResponseEndPoint<IASTransHeadRes>>(jsonString1.Result);
                                                if (resMakeTransAndHold.StatusCode == StatusCode.Success && resMakeTransAndHold.Data != null)
                                                {
                                                    transHeadId = resMakeTransAndHold.Data.TransHeadId;
                                                    // Gắn mã giao dịch chi tiết 
                                                    if (resMakeTransAndHold.Data.ListTransDetail != null && resMakeTransAndHold.Data.ListTransDetail.Count > 0)
                                                    {
                                                        try
                                                        {
                                                            foreach (var itemGiftStockDetail in lstGiftStockDetail)
                                                            {
                                                                itemGiftStockDetail.TRANS_DETAIL_ID = resMakeTransAndHold.Data.ListTransDetail.Where(sp => sp.DetailIdLink == itemGiftStockDetail.DETAIL_ID_LINK).FirstOrDefault().TransDetailId;
                                                                var price = itemGiftStockDetail.PRICE_NO_VAT * (itemGiftStockDetail.VAT / 100) + itemGiftStockDetail.PRICE_NO_VAT;
                                                                totalPayment += Convert.ToDouble(itemGiftStockDetail.QTY_TRANS) * price;
                                                            }

                                                        }
                                                        catch (Exception ex)
                                                        {

                                                            _logger.LogError(ex, "CreateLogAsync Exception: " + ex.Message);
                                                            throw ex;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    iTrans.Rollback();
                                                    return new ResponseError(StatusCode.Fail, resMakeTransAndHold.Message);
                                                }
                                            }
                                        }

                                        // Khởi tạo RefNo theo qui tắc
                                        // detail.RefNumber = string.Join("", new string[3] { "CTKM", DateTime.Now.ToString("ddMMyy"), Guid.NewGuid().ToString().Substring(0, 4).ToUpper() });
                                        detail.RefNumber = "";
                                        // Nếu là hình thức "Tiền mặt" tài khoản nhận là TKTGDVKH lấy từ config hệ thống
                                        if (detail.GiftForm == "Tiền mặt")
                                        {
                                            detail.ToAccount = _tKTGDVKH;
                                            detail.PosCd = model.Pos;
                                        }

                                        var dyParam1 = new OracleDynamicParameters();
                                        dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                        dyParam1.Add("P_PRODUCTINSTANCEID", product.ProductInstanceId, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_TRANSACTIONID", transactionId, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_PROMOTIONID", product.PromotionId, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_PROMOTIONNAME", product.PromotionName, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTFORM", detail.GiftForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTNAME", detail.GiftName, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_QUANTITY", detail.Quantity, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTVALUE", detail.GiftValue, OracleMappingType.Decimal, ParameterDirection.Input);
                                        // dyParam1.Add("P_REFNUMBER", detail.RefNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTCASHVALUE", detail.GiftCashValue, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_IASCODEREFERENCE", detail.IasCodeReference, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_FROMACCOUNT", detail.FromAccount, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_TOACCOUNT", detail.ToAccount, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_TRANSACTIONTYPE", detail.TransactionType, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_POSCD", detail.PosCd, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_TOTALPAYMENT", totalPayment, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_IASTRANSHEADID", transHeadId, OracleMappingType.Decimal, ParameterDirection.Input);

                                        _logger.LogInformation("CreateLogAsync BEFORE PKG_PRM_TRANSACTION_LOG.CREATE_TRANSACTION_LOGDETAIL" + JsonConvert.SerializeObject(product) + " - p: " + JsonConvert.SerializeObject(detail));
                                        _logger.LogInformation("CreateLogAsync P_IASTRANSHEADID " + transHeadId + " - P_TOTALPAYMENT " + totalPayment);

                                        var executeResult = await _prmTransactionLogHandler.ExecuteProcOracle(procName1, iConn, iTrans, dyParam1) as ResponseObject<ResponseModel>;

                                        _logger.LogInformation("CreateLogAsync PKG_PRM_TRANSACTION_LOG.CREATE_TRANSACTION_LOGDETAIL" + JsonConvert.SerializeObject(executeResult));

                                        if (executeResult.StatusCode == StatusCode.Fail && executeResult.Data != null)
                                        {
                                            iTrans.Rollback();
                                            return new ResponseError(StatusCode.Fail, executeResult.Message);
                                        }
                                        else
                                        {
                                            var logDetailId = executeResult.Data.Id;

                                            if (lstGiftStockDetail.Count > 0)
                                            {
                                                foreach (var itemGiftStockDetail in lstGiftStockDetail)
                                                {
                                                    var procName2 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.ADD_GIFT_STOCK_DETAIL");
                                                    var dyParam2 = new OracleDynamicParameters();
                                                    dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                                    dyParam2.Add("P_LOGDETAILID", logDetailId, OracleMappingType.Decimal, ParameterDirection.Input);
                                                    dyParam2.Add("P_IASCODEREFERENCE", itemGiftStockDetail.ITEM_ID.ToString(), OracleMappingType.Varchar2, ParameterDirection.Input);
                                                    dyParam2.Add("P_DETAILIDLINK", itemGiftStockDetail.DETAIL_ID_LINK, OracleMappingType.Decimal, ParameterDirection.Input);
                                                    dyParam2.Add("P_IASTRANSDETAILID", itemGiftStockDetail.TRANS_DETAIL_ID, OracleMappingType.Decimal, ParameterDirection.Input);
                                                    dyParam2.Add("P_QUANTITY", itemGiftStockDetail.QTY_TRANS, OracleMappingType.Varchar2, ParameterDirection.Input);
                                                    dyParam2.Add("P_PRICE", itemGiftStockDetail.PRICE_NO_VAT, OracleMappingType.Varchar2, ParameterDirection.Input);
                                                    _logger.LogInformation("CreateLogAsync BEFORE PKG_PRM_TRANSACTION_LOG.ADD_GIFT_STOCK_DETAIL" + JsonConvert.SerializeObject(itemGiftStockDetail));
                                                    var executeResult1 = await _prmTransactionLogHandler.ExecuteProcOracle(procName2, iConn, iTrans, dyParam2) as ResponseObject<ResponseModel>;
                                                    _logger.LogInformation("CreateLogAsync PKG_PRM_TRANSACTION_LOG.ADD_GIFT_STOCK_DETAIL" + JsonConvert.SerializeObject(executeResult1));
                                                    if (executeResult1.StatusCode == StatusCode.Fail && executeResult1.Data != null)
                                                    {
                                                        iTrans.Rollback();
                                                        return new ResponseError(StatusCode.Fail, executeResult1.Message);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        iTrans.Rollback();
                                        if (_logger != null)
                                        {
                                            _logger.LogError(ex, "Exception Error");
                                            return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ!");
                                        }
                                        throw ex;
                                    }
                                }
                            }
                        }
                    }
                    iTrans.Commit();
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
                throw ex;
            }
        }
        public async Task<Response> UpdateLogAsync(decimal transactionId, List<PrmProductTransactionLogCreateModel> listPrmProductTransactionLogModel)
        {
            try
            {
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();

                    // Lấy thông tin transaction log
                    var procName0 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_TRANSACTION_LOG_BY_ID");
                    var dyParam0 = new OracleDynamicParameters();
                    dyParam0.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam0.Add("P_ID", transactionId, OracleMappingType.Decimal, ParameterDirection.Input);

                    var getTransactionLogById = await _prmTransactionLogHandler.ExecuteProcOracleReturnRow(procName0, dyParam0, false) as ResponseObject<PrmTransactionLogModel>;

                    // Xóa các log detail theo transaction id
                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.DELETE_DETAIL_BY_TRANSACTION");
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_TRANSACTIONID", transactionId, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_TYPE", 0, OracleMappingType.Decimal, ParameterDirection.Input);

                    var result = await _prmTransactionLogHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;

                    if (getTransactionLogById.StatusCode == StatusCode.Success && result.StatusCode == StatusCode.Success)
                    {
                        var transLog = getTransactionLogById.Data;
                        // Thêm Log detail
                        if (listPrmProductTransactionLogModel != null && listPrmProductTransactionLogModel.Count > 0 && transLog.IsInDebt != 1)
                        {
                            foreach (var product in listPrmProductTransactionLogModel)
                            {
                                var procName1 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CREATE_TRANSACTION_LOGDETAIL");
                                foreach (var detail in product.ListPrmTransactionLogDetailModel)
                                {
                                    try
                                    {
                                        var lstGiftStockDetail = new List<IAS_NVLItemDetailModel>();
                                        double totalPayment = 0;
                                        decimal transHeadId = 0;
                                        if (detail.IasCodeReference != "0")
                                        {
                                            // Lấy chi tiết hàng theo kho
                                            using (var client = new HttpClient())
                                            {
                                                string getIasStockOnHandUri = _urlIasApiEndpoint + "api/ias/nvl/get-stock-onhand?listItemId=" + detail.IasCodeReference;
                                                client.DefaultRequestHeaders.Clear();
                                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                                                client.DefaultRequestHeaders.Add("X-PosCd", transLog.Pos);
                                                client.DefaultRequestHeaders.Add("X-DeptId", transLog.DeptId);

                                                var task = await client.GetAsync(getIasStockOnHandUri);
                                                var jsonString = task.Content.ReadAsStringAsync();

                                                ResponseEndPointList<IAS_NVLItemDetailModel> resGetStockOnHand = JsonConvert.DeserializeObject<ResponseEndPointList<IAS_NVLItemDetailModel>>(jsonString.Result);
                                                if (resGetStockOnHand == null || resGetStockOnHand.StatusCode == StatusCode.Fail || resGetStockOnHand.Data.Count == 0) return new ResponseError(StatusCode.Fail, "Không lấy được thông tin tồn kho của quà tặng!");

                                                // Phân bổ hàng hóa lấy theo lô
                                                decimal slCanLay = detail.Quantity;
                                                decimal slDaLay = 0;
                                                decimal slConLai = 0;
                                                var allStockHasItem = resGetStockOnHand.Data.OrderBy(sp => sp.DETAIL_ID_LINK).ToList();
                                                for (int i = 0; i < allStockHasItem.Count; i++)
                                                {
                                                    slConLai = slCanLay - slDaLay;
                                                    var item = allStockHasItem[i];
                                                    if (slConLai <= 0) break;
                                                    if (slConLai <= item.QTY_TRANS)
                                                    {
                                                        slDaLay += slConLai;
                                                        lstGiftStockDetail.Add(new IAS_NVLItemDetailModel
                                                        {
                                                            ITEM_ID = item.ITEM_ID,
                                                            TRANS_DETAIL_ID = 0,
                                                            DETAIL_ID_LINK = item.DETAIL_ID_LINK,
                                                            PRICE_NO_VAT = item.PRICE_NO_VAT,
                                                            QTY_TRANS = slConLai
                                                        });
                                                        break;
                                                    }
                                                    if (slConLai > item.QTY_TRANS)
                                                    {
                                                        slDaLay += item.QTY_TRANS;
                                                        lstGiftStockDetail.Add(new IAS_NVLItemDetailModel
                                                        {
                                                            ITEM_ID = item.ITEM_ID,
                                                            TRANS_DETAIL_ID = 0,
                                                            DETAIL_ID_LINK = item.DETAIL_ID_LINK,
                                                            PRICE_NO_VAT = item.PRICE_NO_VAT,
                                                            QTY_TRANS = item.QTY_TRANS
                                                        });
                                                    }
                                                }
                                                // Tạo phiếu xuất kho và giữ hàng
                                                string makeTransAndHoldUri = _urlIasApiEndpoint + "api/ias/nvl/trans-and-hold";
                                                client.DefaultRequestHeaders.Clear();
                                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
                                                client.DefaultRequestHeaders.Add("X-PosCd", transLog.Pos);
                                                client.DefaultRequestHeaders.Add("X-DeptId", transLog.DeptId);

                                                var exportTransCreateModel = new IAS_ExportTransCreateModel
                                                {
                                                    DeptCd = transLog.DeptId,
                                                    PosCd = transLog.Pos,
                                                    PromoId = product.PromotionId,
                                                    UserName = transLog.UserStaff,
                                                    ItemRequest = lstGiftStockDetail
                                                };

                                                var jsonQueryModel = JsonConvert.SerializeObject(exportTransCreateModel);
                                                var buffer = Encoding.UTF8.GetBytes(jsonQueryModel);
                                                var byteContent = new ByteArrayContent(buffer);
                                                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                                var task1 = await client.PostAsync(makeTransAndHoldUri, byteContent);
                                                var jsonString1 = task1.Content.ReadAsStringAsync();

                                                ResponseEndPoint<IASTransHeadRes> resMakeTransAndHold = JsonConvert.DeserializeObject<ResponseEndPoint<IASTransHeadRes>>(jsonString1.Result);
                                                if (resMakeTransAndHold.StatusCode == StatusCode.Success && resMakeTransAndHold.Data != null)
                                                {
                                                    transHeadId = resMakeTransAndHold.Data.TransHeadId;
                                                    // Gắn mã giao dịch chi tiết 
                                                    if (resMakeTransAndHold.Data.ListTransDetail != null && resMakeTransAndHold.Data.ListTransDetail.Count > 0)
                                                    {
                                                        try
                                                        {
                                                            foreach (var itemGiftStockDetail in lstGiftStockDetail)
                                                            {
                                                                itemGiftStockDetail.TRANS_DETAIL_ID = resMakeTransAndHold.Data.ListTransDetail.Where(sp => sp.DetailIdLink == itemGiftStockDetail.DETAIL_ID_LINK).FirstOrDefault().TransDetailId;
                                                                var price = itemGiftStockDetail.PRICE_NO_VAT * (itemGiftStockDetail.VAT / 100) + itemGiftStockDetail.PRICE_NO_VAT;
                                                                totalPayment += Convert.ToDouble(itemGiftStockDetail.QTY_TRANS) * price;
                                                            }

                                                        }
                                                        catch (Exception)
                                                        {
                                                            throw;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    iTrans.Rollback();
                                                    return new ResponseError(StatusCode.Fail, "Tạo giao dịch xuất kho thất bại!");
                                                }
                                            }
                                        }

                                        // Khởi tạo RefNo theo qui tắc
                                        detail.RefNumber = string.Join("", new string[2] { "CTKM", DateTime.Now.ToString("ddMMyyHHmmss") });

                                        var dyParam1 = new OracleDynamicParameters();
                                        dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                        dyParam1.Add("P_PRODUCTINSTANCEID", product.ProductInstanceId, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_TRANSACTIONID", transactionId, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_PROMOTIONID", product.PromotionId, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_PROMOTIONNAME", product.PromotionName, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTFORM", detail.GiftForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTNAME", detail.GiftName, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_QUANTITY", detail.Quantity, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTVALUE", detail.GiftValue, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_REFNUMBER", detail.RefNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_GIFTCASHVALUE", detail.GiftCashValue, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_IASCODEREFERENCE", detail.IasCodeReference, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_FROMACCOUNT", detail.FromAccount, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_TOACCOUNT", detail.ToAccount, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_TRANSACTIONTYPE", detail.TransactionType, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_POSCD", detail.PosCd, OracleMappingType.Varchar2, ParameterDirection.Input);
                                        dyParam1.Add("P_TOTALPAYMENT", totalPayment, OracleMappingType.Decimal, ParameterDirection.Input);
                                        dyParam1.Add("P_IASTRANSHEADID", transHeadId, OracleMappingType.Decimal, ParameterDirection.Input);

                                        var executeResult = await _prmTransactionLogHandler.ExecuteProcOracle(procName1, iConn, iTrans, dyParam1) as ResponseObject<ResponseModel>;
                                        if (executeResult.StatusCode == StatusCode.Fail && executeResult.Data != null)
                                        {
                                            iTrans.Rollback();
                                            return new ResponseError(StatusCode.Fail, executeResult.Message);
                                        }
                                        else
                                        {
                                            var logDetailId = executeResult.Data.Id;

                                            if (lstGiftStockDetail.Count > 0)
                                            {
                                                foreach (var itemGiftStockDetail in lstGiftStockDetail)
                                                {
                                                    var procName2 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.ADD_GIFT_STOCK_DETAIL");
                                                    var dyParam2 = new OracleDynamicParameters();
                                                    dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                                    dyParam2.Add("P_LOGDETAILID", logDetailId, OracleMappingType.Decimal, ParameterDirection.Input);
                                                    dyParam2.Add("P_IASCODEREFERENCE", itemGiftStockDetail.ITEM_ID.ToString(), OracleMappingType.Varchar2, ParameterDirection.Input);
                                                    dyParam2.Add("P_DETAILIDLINK", itemGiftStockDetail.DETAIL_ID_LINK, OracleMappingType.Decimal, ParameterDirection.Input);
                                                    dyParam2.Add("P_IASTRANSDETAILID", itemGiftStockDetail.TRANS_DETAIL_ID, OracleMappingType.Decimal, ParameterDirection.Input);
                                                    dyParam2.Add("P_QUANTITY", itemGiftStockDetail.QTY_TRANS, OracleMappingType.Varchar2, ParameterDirection.Input);
                                                    dyParam2.Add("P_PRICE", itemGiftStockDetail.PRICE_NO_VAT, OracleMappingType.Varchar2, ParameterDirection.Input);

                                                    var executeResult1 = await _prmTransactionLogHandler.ExecuteProcOracle(procName2, iConn, iTrans, dyParam2) as ResponseObject<ResponseModel>;
                                                    if (executeResult1.StatusCode == StatusCode.Fail && executeResult1.Data != null)
                                                    {
                                                        iTrans.Rollback();
                                                        return new ResponseError(StatusCode.Fail, executeResult1.Message);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        iTrans.Rollback();
                                        if (_logger != null)
                                        {
                                            _logger.LogError(ex, "Exception Error");
                                            return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ!");
                                        }
                                        throw ex;
                                    }
                                }
                            }

                            iTrans.Commit();
                        }
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
                throw ex;
            }
        }
        public async Task<Response> ApproveByIdAsync(decimal id, string approvedBy)
        {
            try
            {
                using (var unitOfWorkOracle = new UnitOfWorkOracle())
                {
                    var iConn = unitOfWorkOracle.GetConnection();
                    var iTrans = iConn.BeginTransaction();

                    // Kiểm tra hạn mức chương trình
                    var procName0 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.VALIDATE_LIMIT_PROMOTION");
                    var dyParam0 = new OracleDynamicParameters();
                    dyParam0.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam0.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    _logger.LogInformation("ApproveByIdAsync before result0: " + id);
                    var result0 = await _prmTransactionLogHandler.ExecuteProcOracle(procName0, iConn, iTrans, dyParam0) as ResponseObject<ResponseModel>;
                    _logger.LogInformation("ApproveByIdAsync result0: " + JsonConvert.SerializeObject(result0));
                    if (result0.StatusCode == StatusCode.Fail && result0.Data != null)
                    {
                        if (result0.Data.Name == "HAN_MUC_CTKM_KHONG_DU") return new ResponseObject<ResponseModel>(null, "Hạn mức chương trình khuyến mãi không đủ", StatusCode.Fail);
                        else return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                    }

                    // Kiểm tra hạn mức khách hàng 
                    var procName1 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.VALIDATE_LIMIT_CUSTOMER");
                    var dyParam1 = new OracleDynamicParameters();
                    dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam1.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                    var result1 = await _prmTransactionLogHandler.ExecuteProcOracle(procName1, iConn, iTrans, dyParam1) as ResponseObject<ResponseModel>;
                    _logger.LogInformation("ApproveByIdAsync result1: " + JsonConvert.SerializeObject(result1));

                    if (result1.StatusCode == StatusCode.Fail && result1.Data != null)
                    {
                        if (result1.Data.Name == "VUOT_HAN_MUC_KH") return new ResponseObject<ResponseModel>(null, "Vượt hạn mức cho 1 khách hàng", StatusCode.Fail);
                        else return new ResponseError(StatusCode.Fail, "Lỗi ngoại lệ");
                    }

                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.APPROVE_TRANSACTION_LOG");
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_APPROVEDBY", approvedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var result = await _prmTransactionLogHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                    _logger.LogInformation("ApproveByIdAsync result: " + JsonConvert.SerializeObject(result));
                    if (result != null && result.StatusCode == StatusCode.Success)
                    {
                        #region 1. Lấy danh sách bút toán
                        List<FinancialPostingRequestModel> lstFinancialPostingReqModel = new List<FinancialPostingRequestModel>();
                        ResponseObject<List<AccountingModel>> listGroupAccounting = await GetListAccountingByTransactionId(id) as ResponseObject<List<AccountingModel>>;
                        _logger.LogInformation("ApproveByIdAsync listGroupAccounting: " + JsonConvert.SerializeObject(listGroupAccounting));
                        var lstAccounting = new List<AccountingDetailModel>();
                        if (listGroupAccounting != null && listGroupAccounting.StatusCode == StatusCode.Success && listGroupAccounting.Data.Count > 0)
                        {

                            foreach (var group in listGroupAccounting.Data)
                                lstAccounting.AddRange(group.ListAccountingDetailModel);
                            foreach (var accounting in lstAccounting) lstFinancialPostingReqModel.Add(InitFinancialPosting(accounting));
                        }
                        #endregion
                        #region 2. Gọi Api hạch toán core
                        var lstFinancialPostingResponseModel = new List<FinancialPostingResponseModel>();
                        if (lstFinancialPostingReqModel.Count == 0) return new ResponseError(StatusCode.Fail, "Không có giao dịch hạch toán");
                        // Chạy hạch toán danh sách giao dịch và ghi nhận kết quả
                        foreach (var posting in lstFinancialPostingReqModel)
                        {
                            var resPosting = await ExecFinancialPosting(posting) as FinancialPostingResponseModel;
                            lstFinancialPostingResponseModel.Add(resPosting);
                        }
                        #endregion

                        #region 3. Cập nhật kết quả hạch toán
                        var listRefNo = new List<string>();
                        if (lstFinancialPostingResponseModel.Count > 0)
                        {
                            //var procName2 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.UPDATE_TRANSACTION_LOGDETAIL");
                            var procName3 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CREATE_FINANCIAL_POSTING");                            
                            foreach (var postingRes in lstFinancialPostingResponseModel)
                            {
                                try
                                {
                                    var transType = "PAYMENT";
                                    if (postingRes.logDetailId == 0)
                                    {
                                        postingRes.toAccount = _tKTTNCN;
                                        transType = "PAY_TAX";
                                    }

                                    // Log lại bút toán thanh toán PAYMENT
                                    var dyParam3 = new OracleDynamicParameters();
                                    dyParam3.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                    dyParam3.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                                    dyParam3.Add("P_TRANSACTIONDETAILID", postingRes.logDetailId, OracleMappingType.Decimal, ParameterDirection.Input);
                                    dyParam3.Add("P_FROMACCOUNT", postingRes.fromAccount, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    dyParam3.Add("P_TOACCOUNT", postingRes.toAccount, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    dyParam3.Add("P_AMOUNT", postingRes.Amount, OracleMappingType.Decimal, ParameterDirection.Input);
                                    dyParam3.Add("P_TRANSACTIONTYPE", transType, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    dyParam3.Add("P_REFNO", postingRes.refNo, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    dyParam3.Add("P_POSTINGRESULTCODE", postingRes.errorCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    dyParam3.Add("P_POSTINGRESULTDESC", postingRes.errorDesc, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    dyParam3.Add("P_REMARKS", postingRes.remarks, OracleMappingType.Varchar2, ParameterDirection.Input);

                                    //var dyParam2 = new OracleDynamicParameters();
                                    //dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                                    //dyParam2.Add("P_ID", postingRes.logDetailId, OracleMappingType.Decimal, ParameterDirection.Input);
                                    //dyParam2.Add("P_REFNUMBER", postingRes.refNo, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    //dyParam2.Add("P_FROMACCOUNT", postingRes.fromAccount, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    //dyParam2.Add("P_TRANSACTIONTYPE", "PAYMENT", OracleMappingType.Varchar2, ParameterDirection.Input);
                                    //dyParam2.Add("P_REFNO", postingRes.refNo, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    //dyParam2.Add("P_POSTINGRESULTCODE", postingRes.errorCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    //dyParam2.Add("P_POSTINGRESULTDESC", postingRes.errorDesc, OracleMappingType.Varchar2, ParameterDirection.Input);
                                    //dyParam2.Add("P_REMARKS", postingRes.remarks, OracleMappingType.Varchar2, ParameterDirection.Input);

                                    var executeResult = await _prmTransactionLogHandler.ExecuteProcOracle(procName3, iConn, iTrans, dyParam3) as ResponseObject<ResponseModel>;
                                    listRefNo.Add(postingRes.refNo);
                                    _logger.LogInformation("ApproveByIdAsync executeResult: " + JsonConvert.SerializeObject(executeResult));
                                    if (executeResult.StatusCode == StatusCode.Fail && executeResult.Data != null)
                                    {
                                        iTrans.Rollback();
                                        return new ResponseError(StatusCode.Fail, executeResult.Message);
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
                                    throw ex;
                                }
                            }
                        }
                        #endregion

                        #region 4. Cập nhật lại trạng thái đã phê duyệt cho phiếu xuất kho IAS cho giao dịch "Quà tặng" hạch toàn thành công
                        var listTransHasGift = lstFinancialPostingResponseModel.Where(sp => sp.IasTransHeadId > 0 && sp.errorCode == "0").ToList();
                        if (listTransHasGift != null && listTransHasGift.Count > 0)
                        {
                            foreach (var itemTransHasGift in listTransHasGift)
                            {
                                using (var client = new HttpClient())
                                {
                                    string approveTransHeadUri = _urlIasApiEndpoint + "api/ias/nvl/approve-trans-head";
                                    client.DefaultRequestHeaders.Clear();
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                                    var iASApproveTransHeadModel = new IASApproveTransHeadModel
                                    {
                                        ApprovedBy = approvedBy,
                                        RefNo = itemTransHasGift.refNo,
                                        TransHeadId = itemTransHasGift.IasTransHeadId
                                    };                                    
                                    var jsonQueryModel = JsonConvert.SerializeObject(iASApproveTransHeadModel);
                                    _logger.LogInformation("ApproveByIdAsync api/ias/nvl/approve-trans-head: " + jsonQueryModel);
                                    var buffer = Encoding.UTF8.GetBytes(jsonQueryModel);
                                    var byteContent = new ByteArrayContent(buffer);
                                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                    var task = await client.PostAsync(approveTransHeadUri, byteContent);
                                    var jsonString = task.Content.ReadAsStringAsync();

                                    _logger.LogInformation("ApproveByIdAsync jsonString: " + jsonString);
                                    ResponseEndPoint<IASApproveTransHeadModel> resApproveTransHead = JsonConvert.DeserializeObject<ResponseEndPoint<IASApproveTransHeadModel>>(jsonString.Result);
                                }
                            }
                        }
                        #endregion

                        iTrans.Commit();
                        result.Message = string.Join(',', listRefNo);
                    }
                    else iTrans.Rollback();
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
                throw ex;
            }
        }
        public async Task<Response> ResendByIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.RESEND_TRANSACTION_LOG");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _prmTransactionLogHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.DELETE_TRANSACTION_LOG");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _prmTransactionLogHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
        public async Task<Response> RevokeByIdAsync(decimal id, string accRevoke, string status, string revokedBy, string revokeComment, string posRevoke, string userStaffRevoke)
        {
            try
            {
                if (status == "REVOKED")
                {
                    var accountNumber = "";
                    var posAccountNumber = "";
                    var promoCostAccount = "";
                    var saveAccountNumber = "";
                    decimal totalRevokeValue = 0;
                    var cifNumber = "";
                    var custName = "";
                    var posCd = "";
                    var listRevokeFinancialPostingModel = new List<AccountingDetailModel>();
                    var getTransById = await GetByIdAsync(id) as ResponseObject<PrmTransactionLogModel>;
                    if (getTransById == null || getTransById.StatusCode == StatusCode.Fail)
                        return new ResponseError(StatusCode.Fail, "Không truy vấn được thông tin giao dịch!");
                    accountNumber = getTransById.Data.AccountNumber.Split("|")[0];
                    posAccountNumber = getTransById.Data.AccountNumber.Split("|")[1];
                    totalRevokeValue = getTransById.Data.TotalGiftValue;
                    cifNumber = getTransById.Data.CifNumber;
                    custName = getTransById.Data.CustomerName;
                    posCd = getTransById.Data.Pos;
                    saveAccountNumber = getTransById.Data.SaveAccountNumber;
                    //1. Kiểm tra tài khoản thanh toán có đủ số dư khả dụng hay không? đối với trường hợp là TKTT của KH không phải là TKTGDVKH
                    if (accountNumber != _tKTGDVKH)
                    {
                        var listAccByCust = new List<AccountModel>();
                        string requestGetAccountByCifUri = _urlEsbGwApiEndpoint + "api/v1/account/get_account_list_by_cif";
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                            // Data
                            var accountQueryModel = new AccountQueryModel
                            {
                                AccountType = "001",
                                CustType = "CIF",
                                CustomerId = cifNumber
                            };

                            var jsonQueryModel = JsonConvert.SerializeObject(accountQueryModel);
                            var buffer = Encoding.UTF8.GetBytes(jsonQueryModel);
                            var byteContent = new ByteArrayContent(buffer);
                            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var task = await client.PostAsync(requestGetAccountByCifUri, byteContent);
                            var jsonString = task.Content.ReadAsStringAsync();
                            ResponseAccountModel responseGetAccountByCifUri = JsonConvert.DeserializeObject<ResponseAccountModel>(jsonString.Result);
                            if (responseGetAccountByCifUri.ErrorCode == "0")
                                listAccByCust = responseGetAccountByCifUri.ListAccounts;
                        }
                        if (listAccByCust.Count == 0) return new ResponseError(StatusCode.Fail, "Không truy vấn được thông tin tài khoản khách hàng!");
                        var avaiBal = listAccByCust.Where(sp => sp.AccountNum == accountNumber).FirstOrDefault() != null ? listAccByCust.Where(sp => sp.AccountNum == accountNumber).FirstOrDefault().AvailBal : "0";
                        var avaiBalParse = Convert.ToDecimal(avaiBal);
                        if (avaiBalParse < totalRevokeValue) return new ResponseError(StatusCode.Fail, "TK đang chọn không đủ số dư khả dụng để thu hồi, vui lòng nạp tiền vào TK!");

                    }


                    //2. Hạch toán tiền từ tktt khách hàng hoặc tài khoản trung gian DVKH vào tài khoản chi phí ctkm
                    //2.0 Lấy danh sách giao dịch chi tiết để thực hiện thu hồi

                    //2.1 Lấy số tk chi phí ctkm, ưu tiền tk loại tiền mặt hoặc chuyển khoản, nếu không có lấy đến tài khoản quà tặng
                    var listTransDetail = new List<PrmTransactionLogDetailModel>();
                    if (getTransById.Data.ListPrmProductTransactionLogModel != null && getTransById.Data.ListPrmProductTransactionLogModel.Count > 0)
                    {
                        foreach (var item in getTransById.Data.ListPrmProductTransactionLogModel)
                        {
                            if (item.ListPrmTransactionLogDetailModel != null && item.ListPrmTransactionLogDetailModel.Count > 0)
                                listTransDetail.AddRange(item.ListPrmTransactionLogDetailModel);
                        }
                    }
                    if (listTransDetail.Count > 0)
                    {
                        var cashAcc = listTransDetail.Where(sp => sp.SpendForm == "Tiền mặt" || sp.SpendForm == "Chuyển khoản").FirstOrDefault();
                        promoCostAccount = cashAcc != null ? cashAcc.FromAccount : listTransDetail.FirstOrDefault().FromAccount;

                        foreach (var transDetail in listTransDetail)
                        {
                            var revokeFinancialPostingModel = new AccountingDetailModel
                            {
                                RefNumber = string.Join("", new string[3] { "CTKM", DateTime.Now.ToString("ddMMyy"), Guid.NewGuid().ToString().Substring(0, 4).ToUpper() }),
                            };

                            // Diễn giải giao dịch
                            revokeFinancialPostingModel.Remarks = "THU HOI KHUYEN MAI THEO SO TK_" + saveAccountNumber + "_" + cifNumber;
                            // Thông tin gửi nhận
                            revokeFinancialPostingModel.FromAccountName = accountNumber == _tKTGDVKH ? "TAI KHOAN TRUNG GIAN DVKH" : custName;
                            revokeFinancialPostingModel.FromAccountNumber = accountNumber;
                            revokeFinancialPostingModel.ToAccountName = "TK Chi phí CTKM";
                            revokeFinancialPostingModel.ToAccountNumber = transDetail.FromAccount;
                            revokeFinancialPostingModel.Amount = transDetail.GiftValue * transDetail.Quantity;
                            revokeFinancialPostingModel.PosCd = posCd;
                            revokeFinancialPostingModel.Pos = posAccountNumber;
                            revokeFinancialPostingModel.LogDetailId = transDetail.Id;
                            listRevokeFinancialPostingModel.Add(revokeFinancialPostingModel);
                        }
                    }
                    //2.2 Lấy bút toán đóng thuế theo giao dịch đang thu hồi
                    var revokeFinancialPostingTaxModel = new AccountingDetailModel();
                    var procName0 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_PAY_TAX_BY_TRANSID");
                    var dyParam0 = new OracleDynamicParameters();
                    dyParam0.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam0.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    var getPayTaxRes = await _financialPostingHandler.ExecuteProcOracleReturnRow(procName0, dyParam0, false) as ResponseObject<PrmFinancialPosting>;
                    if (getPayTaxRes != null && getPayTaxRes.StatusCode == StatusCode.Success)
                    {
                        // Diễn giải giao dịch
                        revokeFinancialPostingTaxModel.Remarks = "THU HOI TNCN THEO SO TK_" + saveAccountNumber + "_" + cifNumber;
                        // Thông tin gửi nhận

                        revokeFinancialPostingTaxModel.FromAccountName = "Tài khoản thuế TNCN";
                        revokeFinancialPostingTaxModel.FromAccountNumber = _tKTTNCN;
                        revokeFinancialPostingTaxModel.ToAccountName = accountNumber == _tKTGDVKH ? "TAI KHOAN TRUNG GIAN DVKH" : custName;
                        revokeFinancialPostingTaxModel.ToAccountNumber = accountNumber;
                        revokeFinancialPostingTaxModel.Amount = getPayTaxRes.Data.Amount;
                        revokeFinancialPostingTaxModel.PosCd = posAccountNumber;
                        revokeFinancialPostingTaxModel.Pos = posCd;
                        revokeFinancialPostingTaxModel.LogDetailId = 0;
                    }

                    if (listRevokeFinancialPostingModel.Count == 0) return new ResponseError(StatusCode.Fail, "Không truy vấn được bút toán để thu hồi!");
                    else
                    {
                        // 2.2 Tạo bút toán 1 ghi nợ, n ghi có
                        var reqRevokePosting = InitOneDebitManyCreditFinPos(listRevokeFinancialPostingModel);
                        var resRevokePosting = await ExecFinancialPosting(reqRevokePosting) as FinancialPostingResponseModel;
                        if (resRevokePosting == null || resRevokePosting.errorCode != "0")
                        {
                            return new ResponseError(StatusCode.Fail, "Hạch toán không thành công!");
                        }
                        foreach (var revokeFinancialPostingModel in listRevokeFinancialPostingModel)
                        {

                            // Log lại bút toán thanh toán REVOKE
                            var procName1 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CREATE_FINANCIAL_POSTING");
                            var dyParam1 = new OracleDynamicParameters();
                            dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam1.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam1.Add("P_TRANSACTIONDETAILID", revokeFinancialPostingModel.LogDetailId, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam1.Add("P_FROMACCOUNT", revokeFinancialPostingModel.FromAccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam1.Add("P_TOACCOUNT", revokeFinancialPostingModel.ToAccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam1.Add("P_AMOUNT", revokeFinancialPostingModel.Amount, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam1.Add("P_TRANSACTIONTYPE", "REVOKE", OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam1.Add("P_REFNO", resRevokePosting.refNo, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam1.Add("P_POSTINGRESULTCODE", resRevokePosting.errorCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam1.Add("P_POSTINGRESULTDESC", resRevokePosting.errorDesc, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam1.Add("P_REMARKS", revokeFinancialPostingModel.Remarks, OracleMappingType.Varchar2, ParameterDirection.Input);

                            _ = await _prmTransactionLogHandler.ExecuteProcOracle(procName1, dyParam1);


                        }
                        // 2.4 Chạy bút toàn hoàn thuế TNCN
                        if (revokeFinancialPostingTaxModel != null && resRevokePosting.errorCode == "0")
                        {
                            var reqRevokeTaxPosting = InitFinancialPosting(revokeFinancialPostingTaxModel);
                            var resRevokeTaxPosting = await ExecFinancialPosting(reqRevokeTaxPosting) as FinancialPostingResponseModel;

                            // Log lại bút toán hoàn lại thuế TNCN
                            var procName2 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CREATE_FINANCIAL_POSTING");
                            var dyParam2 = new OracleDynamicParameters();
                            dyParam2.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                            dyParam2.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam2.Add("P_TRANSACTIONDETAILID", revokeFinancialPostingTaxModel.LogDetailId, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam2.Add("P_FROMACCOUNT", revokeFinancialPostingTaxModel.FromAccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam2.Add("P_TOACCOUNT", revokeFinancialPostingTaxModel.ToAccountNumber, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam2.Add("P_AMOUNT", revokeFinancialPostingTaxModel.Amount, OracleMappingType.Decimal, ParameterDirection.Input);
                            dyParam2.Add("P_TRANSACTIONTYPE", "REVOKE_TAX", OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam2.Add("P_REFNO", resRevokeTaxPosting.refNo, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam2.Add("P_POSTINGRESULTCODE", resRevokeTaxPosting.errorCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam2.Add("P_POSTINGRESULTDESC", resRevokeTaxPosting.errorDesc, OracleMappingType.Varchar2, ParameterDirection.Input);
                            dyParam2.Add("P_REMARKS", revokeFinancialPostingTaxModel.Remarks, OracleMappingType.Varchar2, ParameterDirection.Input);

                            _ = await _prmTransactionLogHandler.ExecuteProcOracle(procName2, dyParam2);
                        }
                    }


                }

                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.REVOKE_TRANSACTION_LOG");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_ACCREVOKE", accRevoke, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STATUS", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_REVOKEDBY", revokedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_REVOKECOMMENT", revokeComment, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_POSREVOKE", posRevoke, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_USERSTAFFREVOKE", userStaffRevoke, OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _prmTransactionLogHandler.ExecuteProcOracle(procName, dyParam);

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
        public async Task<Response> RejectByIdAsync(decimal id, string approvedBy, string approvedComment)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.REJECT_TRANSACTION_LOG");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_APPROVEDBY", approvedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_APPROVECOMMENT", approvedComment, OracleMappingType.Varchar2, ParameterDirection.Input);

                // Hoàn giao dịch giữ quà với loại có tặng quà
                // 1. Lấy danh sách chi tiết quà đã giữ
                RepositoryHandler<PrmGiftStockDetail, PrmGiftStockDetail, PrmPromotionQueryModel> _giftStockDetailHandler
               = new RepositoryHandler<PrmGiftStockDetail, PrmGiftStockDetail, PrmPromotionQueryModel>();

                var procName0 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_GIFT_STOCK_DETAIL");
                var dyParam0 = new OracleDynamicParameters();
                dyParam0.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam0.Add("P_TRANSACTIONID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var getListGiftStockDetail = await _giftStockDetailHandler.ExecuteProcOracleReturnRow(procName0, dyParam0, true) as ResponseObject<List<PrmGiftStockDetail>>;
                if (getListGiftStockDetail != null && getListGiftStockDetail.StatusCode == StatusCode.Success && getListGiftStockDetail.Data.Count > 0)
                {
                    var listItemDetail = new List<IAS_NVLItemDetailModel>();
                    listItemDetail = (from dt in getListGiftStockDetail.Data
                                      select new IAS_NVLItemDetailModel
                                      {
                                          DETAIL_ID_LINK = dt.DetailIdLink,
                                          QTY_TRANS = dt.Quantity
                                      }).ToList();

                    // 2. Gọi service IAS để hoàn giao dịch giữ quà
                    using (var client = new HttpClient())
                    {
                        string revertHoldingItemUri = _urlIasApiEndpoint + "api/ias/nvl/revert-holding";
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                        var jsonQueryModel = JsonConvert.SerializeObject(listItemDetail);
                        var buffer = Encoding.UTF8.GetBytes(jsonQueryModel);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                        var task = await client.PostAsync(revertHoldingItemUri, byteContent);
                        var jsonString = task.Content.ReadAsStringAsync();

                        ResponseEndPoint<IASTransHeadRes> resRevertHoldingItem = JsonConvert.DeserializeObject<ResponseEndPoint<IASTransHeadRes>>(jsonString.Result);
                        if (resRevertHoldingItem.StatusCode == StatusCode.Success && resRevertHoldingItem.Data != null)
                        {
                            var result0 = await _prmTransactionLogHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                            return result0;
                        }
                        else new ResponseError(StatusCode.Fail, resRevertHoldingItem.Message);
                    }
                }
                // Không có giao dịch tặng quà vẫn từ chối bình thường                
                var result = await _prmTransactionLogHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
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
        public async Task<Response> CheckAcctTideIsClosedByTransLog(decimal id,string posCd)
        {
            try
            {
                var getTransLogById = await GetByIdAsync(id) as ResponseObject<PrmTransactionLogModel>;
                if (getTransLogById.StatusCode == StatusCode.Success)
                {
                    // Lấy thông tin sổ tiết kiệm theo số sổ tk và cmnd
                    string requestCoreApiUri = _urlCoreApiEndpoint + "api/core/common/get-acct-tide?legacyRefNo=" + getTransLogById.Data.SaveAccountNumber + "&lincese=" + getTransLogById.Data.License + "";
                    AcctTideInfoModel acctTideInfoModel = new AcctTideInfoModel();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                        var task = await client.GetAsync(requestCoreApiUri);
                        var jsonString = task.Content.ReadAsStringAsync();
                        ResponseAcctTideInfoModel responseAcctTideModel = JsonConvert.DeserializeObject<ResponseAcctTideInfoModel>(jsonString.Result);
                        if (responseAcctTideModel.StatusCode == StatusCode.Success && responseAcctTideModel.Data != null)
                        {
                            if (responseAcctTideModel.Data.Status != "C") return new ResponseObject<PrmTransactionLogModel>(null, "Sổ chưa tất toán, không thể thực hiện thu hồi", StatusCode.Fail);
                            else if (responseAcctTideModel.Data.Status == "C" && responseAcctTideModel.Data.Pos_Close != posCd) return new ResponseObject<PrmTransactionLogModel>(null, "Sổ đã được tất toán tại chi nhánh khác", StatusCode.Fail);
                            else return new ResponseObject<PrmTransactionLogModel>(null, "Sổ đã tất toán", StatusCode.Success);

                        }
                        else return new ResponseObject<PrmTransactionLogModel>(null, "Thông tin sổ tiết kiệm không tồn tại", StatusCode.Fail);
                    }
                }
                return getTransLogById;
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
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        #region Financial Posting
        private FinancialPostingRequestModel InitFinancialPosting(AccountingDetailModel accountingDetailModel)
        {
            var refNo = string.Join("", new string[3] { "CTKM", DateTime.Now.ToString("ddMMyy"), Guid.NewGuid().ToString().Substring(0, 4).ToUpper() });
            // Khởi tạo financial posting
            int segNo = 1;
            var financialPostingReqModel = new FinancialPostingRequestModel
            {
                channelCode = "NET",
                transDate = DateTime.Now.ToString("yyyyMMdd"),
                refNo = refNo,
                //refNo = accountingDetailModel.RefNumber,
                sourceBranchCode = _mainPosCDFinancialPosting,
                listSegInfos = new List<SegInfos>(),
                logDetailId = accountingDetailModel.LogDetailId,
                remarks = accountingDetailModel.Remarks
            };
            // Thêm chân giao dịch ghi có
            financialPostingReqModel.listSegInfos.Add(new SegInfos
            {
                segNo = segNo.ToString(),
                accountName = accountingDetailModel.ToAccountName,
                accountNum = accountingDetailModel.ToAccountNumber,
                accountBranchCode = accountingDetailModel.PosCd,
                debitCreditFlag = "C",
                transAmount = accountingDetailModel.Amount.ToString(),
                transCurrency = "VND",
                extRemark = accountingDetailModel.Remarks
                //extRemark = accountingDetailModel.ToAccountNumber + "(GD#" + financialPostingReqModel.refNo + ")"
            });
            segNo++;
            // Thêm chân giao dịch ghi nợ
            financialPostingReqModel.listSegInfos.Add(new SegInfos
            {
                segNo = segNo.ToString(),
                accountName = accountingDetailModel.FromAccountName,
                accountNum = accountingDetailModel.FromAccountNumber,
                accountBranchCode = accountingDetailModel.Pos,
                debitCreditFlag = "D",
                transAmount = accountingDetailModel.Amount.ToString(),
                transCurrency = "VND",
                extRemark = accountingDetailModel.Remarks
                //extRemark = accountingDetailModel.FromAccountNumber + "(GD#" + financialPostingReqModel.refNo + ")",
            });
            financialPostingReqModel.fromAccount = accountingDetailModel.FromAccountNumber;

            financialPostingReqModel.numOfSeq = financialPostingReqModel.listSegInfos.Count.ToString();
            // Tạo checksum
            string cksum = "";
            foreach (var seg in financialPostingReqModel.listSegInfos)
                cksum += seg.accountNum + seg.transAmount + seg.extRemark;

            cksum += _keyMD5;
            financialPostingReqModel.checkSum = MD5Hash(cksum);
            financialPostingReqModel.IasTransHeadId = accountingDetailModel.IasTransHeadId;
            return financialPostingReqModel;
        }
        private FinancialPostingRequestModel InitOneDebitManyCreditFinPos(List<AccountingDetailModel> lstAccountingDetailModel)
        {
            var refNo = string.Join("", new string[3] { "CTKM", DateTime.Now.ToString("ddMMyy"), Guid.NewGuid().ToString().Substring(0, 4).ToUpper() });
            // Khởi tạo financial posting
            int segNo = 1;
            var financialPostingReqModel = new FinancialPostingRequestModel
            {
                channelCode = "NET",
                transDate = DateTime.Now.ToString("yyyyMMdd"),
                refNo = refNo,
                //refNo = accountingDetailModel.RefNumber,
                sourceBranchCode = _mainPosCDFinancialPosting,
                listSegInfos = new List<SegInfos>(),
                logDetailId = lstAccountingDetailModel[0].LogDetailId,
                remarks = lstAccountingDetailModel[0].Remarks
            };
            // Thêm chân giao dịch ghi nợ
            financialPostingReqModel.listSegInfos.Add(new SegInfos
            {
                segNo = segNo.ToString(),
                accountName = lstAccountingDetailModel[0].FromAccountName,
                accountNum = lstAccountingDetailModel[0].FromAccountNumber,
                accountBranchCode = lstAccountingDetailModel[0].Pos,
                debitCreditFlag = "D",
                transAmount = lstAccountingDetailModel.Sum(sp => sp.Amount).ToString(),
                transCurrency = "VND",
                extRemark = lstAccountingDetailModel[0].Remarks
                //extRemark = accountingDetailModel.FromAccountNumber + "(GD#" + financialPostingReqModel.refNo + ")",
            });
            for (int i = 0; i < lstAccountingDetailModel.Count; i++)
            {
                segNo++;
                // Thêm chân giao dịch ghi có
                financialPostingReqModel.listSegInfos.Add(new SegInfos
                {
                    segNo = segNo.ToString(),
                    accountName = lstAccountingDetailModel[i].ToAccountName,
                    accountNum = lstAccountingDetailModel[i].ToAccountNumber,
                    accountBranchCode = lstAccountingDetailModel[i].PosCd,
                    debitCreditFlag = "C",
                    transAmount = lstAccountingDetailModel[i].Amount.ToString(),
                    transCurrency = "VND",
                    extRemark = lstAccountingDetailModel[i].Remarks
                    //extRemark = accountingDetailModel.ToAccountNumber + "(GD#" + financialPostingReqModel.refNo + ")"
                });
            }
            financialPostingReqModel.fromAccount = lstAccountingDetailModel[0].FromAccountNumber;

            financialPostingReqModel.numOfSeq = financialPostingReqModel.listSegInfos.Count.ToString();
            // Tạo checksum
            string cksum = "";
            foreach (var seg in financialPostingReqModel.listSegInfos)
                cksum += seg.accountNum + seg.transAmount + seg.extRemark;

            cksum += _keyMD5;
            financialPostingReqModel.checkSum = MD5Hash(cksum);
            financialPostingReqModel.IasTransHeadId = lstAccountingDetailModel[0].IasTransHeadId;
            return financialPostingReqModel;
        }
        private async Task<FinancialPostingResponseModel> ExecFinancialPosting(FinancialPostingRequestModel posting)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    _logger.LogInformation("//START Financial Posting");
                    _logger.LogInformation("//RefNo:" + posting.refNo);
                    _logger.LogInformation("//Start time: " + DateTime.Now.ToLongTimeString());

                    await Task.Delay(5000);
                    string financialPostingUri = _urlEsbGwApiEndpoint + "api/v1/fundtransfer/create_financial_posting";
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                    var jsonQueryModel = JsonConvert.SerializeObject(posting);
                    var buffer = Encoding.UTF8.GetBytes(jsonQueryModel);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var task = await client.PostAsync(financialPostingUri, byteContent);
                    var jsonString = task.Content.ReadAsStringAsync();
                    FinancialPostingResponseModel resFinancialPosting = JsonConvert.DeserializeObject<FinancialPostingResponseModel>(jsonString.Result);
                    resFinancialPosting.logDetailId = posting.logDetailId;
                    resFinancialPosting.fromAccount = posting.fromAccount;
                    resFinancialPosting.remarks = posting.remarks;
                    resFinancialPosting.IasTransHeadId = posting.IasTransHeadId;
                    resFinancialPosting.Amount = posting.listSegInfos.Where(sp => sp.debitCreditFlag == "C").Sum(sp => Convert.ToDecimal(sp.transAmount));
                    _logger.LogInformation("//Res Code: " + resFinancialPosting.errorCode);
                    _logger.LogInformation("//End time: " + DateTime.Now.ToString());
                    _logger.LogInformation("//END Financial Posting");
                    return resFinancialPosting;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
