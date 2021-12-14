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
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace API.BussinessLogic
{
    public class PrmPromotionHandler : IPrmPromotionHandler
    {
        private readonly RepositoryHandler<PrmPromotion, PrmPromotionModel, PrmPromotionQueryModel> _prmPromotionHandler
               = new RepositoryHandler<PrmPromotion, PrmPromotionModel, PrmPromotionQueryModel>();
        private readonly RepositoryHandler<PrmProductInstance, PrmProductInstanceModel, PrmProductInstanceQueryModel> _prmProductInstanceHandler
               = new RepositoryHandler<PrmProductInstance, PrmProductInstanceModel, PrmProductInstanceQueryModel>();
        private readonly RepositoryHandler<PrmTransactionLog, PrmTransactionLogModel, PrmTransactionLogQueryModel> _prmTransactionConditionLogHandler
               = new RepositoryHandler<PrmTransactionLog, PrmTransactionLogModel, PrmTransactionLogQueryModel>();
        private readonly RepositoryHandler<PrmTransactionLogDetail, PrmTransactionLogDetailModel, PrmTransactionLogDetailQueryModel> _prmTransactionLogDetailHandler
               = new RepositoryHandler<PrmTransactionLogDetail, PrmTransactionLogDetailModel, PrmTransactionLogDetailQueryModel>();
        private readonly RepositoryHandler<PrmTransactionLog, PrmTransactionLogModel, PrmTransactionLogQueryModel> _prmTransactionLogHandler
       = new RepositoryHandler<PrmTransactionLog, PrmTransactionLogModel, PrmTransactionLogQueryModel>();
        private readonly string _urlCoreApiEndpoint;
        private readonly string _urlUtilApiEndpoint;
        private readonly string _accessToken;
        private readonly string _dBSchemaName;
        private readonly string _tKTGDVKH;
        private readonly int _thoiGianXacDinhKHMoi;
        private readonly ILogger<PrmPromotionHandler> _logger;
        private PrmProductInstanceHandler prmProductInstanceHandler;

        public PrmPromotionHandler(ILogger<PrmPromotionHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
            _urlCoreApiEndpoint = Helpers.GetConfig("EndPoint:CoreApi:UrlEndpoint");
            _urlUtilApiEndpoint = Helpers.GetConfig("EndPoint:UtilApi:UrlEndpoint");
            _tKTGDVKH = Helpers.GetConfig("PromotionConfig:TKTGDVKH");
            _thoiGianXacDinhKHMoi = Helpers.IsNumber(Helpers.GetConfig("PromotionConfig:ThoiGianXacDinhKHMoi")) ? Convert.ToInt32(Helpers.GetConfig("PromotionConfig:ThoiGianXacDinhKHMoi")) : 12;
            _accessToken = "";
        }
        #region GET
        public async Task<Response> GetByFilterAsync(PrmPromotionQueryModel queryModel)
        {
            try
            {

                var is99Dept = !string.IsNullOrEmpty(queryModel.DeptId) && queryModel.DeptId.Substring(4, 2) == "99";
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.GET_BY_FILTER");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PAGE_SIZE", queryModel.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_PAGE_INDEX", queryModel.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_FULL_TEXT_SEARCH", queryModel.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STATUS", queryModel.Status != "ALL" ? queryModel.Status : "", OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ISREGISTERIAT", queryModel.IsRegisterIAT, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_DEPTID", queryModel.DeptId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ROOTDEPT", is99Dept ? queryModel.DeptId.Substring(0, 4) : "", OracleMappingType.Varchar2, ParameterDirection.Input);

                return await _prmPromotionHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.GET_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _prmPromotionHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);
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
        public async Task<Response> GetListPromoValidByDate(DateTime validDate)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.GET_LIST_PROMO_VALID_BY_DATE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_VAL_DT", validDate, OracleMappingType.Date, ParameterDirection.Input);

                return await _prmPromotionHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetListPromoValidByTransLog(decimal transLogId)
        {
            try
            {
                string legacyRefNo = "";
                string license = "";
                string employeePosCd = "";
                string phone = "";
                string approvedComment = "";
                var result = new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), string.Empty, StatusCode.Success);
                // Lấy Trans Log theo id
                var procName3 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_TRANSACTION_LOG_BY_ID");
                var dyParam3 = new OracleDynamicParameters();
                dyParam3.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam3.Add("P_ID", transLogId, OracleMappingType.Decimal, ParameterDirection.Input);

                var getTransactionLogById = await _prmTransactionLogHandler.ExecuteProcOracleReturnRow(procName3, dyParam3, false) as ResponseObject<PrmTransactionLogModel>;
                if (getTransactionLogById != null && getTransactionLogById.StatusCode == StatusCode.Success)
                {
                    legacyRefNo = getTransactionLogById.Data.SaveAccountNumber;
                    license = getTransactionLogById.Data.License;
                    employeePosCd = getTransactionLogById.Data.Pos;
                    phone = getTransactionLogById.Data.Pos;
                    approvedComment = getTransactionLogById.Data.ApprovedComment;

                    result.Data.TaiKhoanTrungGianDVKH = _tKTGDVKH;
                    // Kiểm tra sổ đã được tạo giao dịch tặng quà chưa
                    var procName0 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CHECK_DEPOSIT_EXIST");
                    var dyParam0 = new OracleDynamicParameters();
                    dyParam0.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam0.Add("P_SAVEACCOUNTNUMBER", legacyRefNo, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var checkExistDepositNo = await _prmTransactionConditionLogHandler.ExecuteProcOracleReturnRow(procName0, dyParam0, false) as ResponseObject<PrmTransactionLogModel>;
                    if (checkExistDepositNo != null && checkExistDepositNo.StatusCode == StatusCode.Success)
                    {
                        return new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), "Sổ này đang có giao dịch 'Chờ Phê Duyệt', vui lòng hoàn thành trước khi tạo giao dịch mới!", StatusCode.Fail);
                    }
                    // Lấy thông tin sổ tiết kiệm theo số sổ tk và cmnd
                    string requestCoreApiUri = _urlCoreApiEndpoint + "api/core/common/get-acct-tide?legacyRefNo=" + legacyRefNo + "&lincese=" + license + "";
                    AcctTideInfoModel acctTideInfoModel = new AcctTideInfoModel();
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                        var task = await client.GetAsync(requestCoreApiUri);
                        var jsonString = task.Content.ReadAsStringAsync();
                        ResponseAcctTideInfoModel responseAcctTideModel = JsonConvert.DeserializeObject<ResponseAcctTideInfoModel>(jsonString.Result);
                        if (responseAcctTideModel.StatusCode == StatusCode.Success && responseAcctTideModel.Data != null && responseAcctTideModel.Data.Status != "C") acctTideInfoModel = responseAcctTideModel.Data;
                        else return new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), "Thông tin sổ tiết kiệm không tồn tại hoặc đã tất toán", StatusCode.Fail);
                    }
                    // Lấy các sản phẩm thuộc chương trình khuyến mãi có điều kiện phù hợp
                    // 1. Theo thời gian mở sổ, theo mã sản phẩm, theo chi nhánh mở sổ
                    var lstPrmProductInstanceValid = new List<PrmProductInstanceModel>();
                    if (acctTideInfoModel != null)
                    {
                        if (!string.IsNullOrEmpty(employeePosCd) && acctTideInfoModel.Pos_CD != employeePosCd) return new ResponseObject<PrmPromotionViewModel>(null, "Sổ tiết kiệm không được mở tại chi nhánh này", StatusCode.Fail);
                        if (!acctTideInfoModel.Val_Dt.HasValue || string.IsNullOrEmpty(acctTideInfoModel.Prod_CD)
                            || string.IsNullOrEmpty(acctTideInfoModel.Pos_CD))
                            return new ResponseObject<PrmPromotionViewModel>(null, "Không lấy được thông tin ngày mở sổ, mã sản phẩm, chi nhánh mở sổ", StatusCode.Fail);
                        // Gán thông tin sổ tiết kiệm
                        result.Data.Branch = acctTideInfoModel.Pos_CD;
                        result.Data.CifNo = acctTideInfoModel.Cif_No;
                        result.Data.DepositNo = acctTideInfoModel.Deposit_No;
                        result.Data.DepositVal = acctTideInfoModel.Avail_Bal;
                        result.Data.Tenure = acctTideInfoModel.Tenure;
                        result.Data.ValidDate = acctTideInfoModel.Val_Dt.Value.ToString("dd/MM/yyyy");
                        result.Data.DueDate = acctTideInfoModel.Mat_Dt.Value.ToString("dd/MM/yyyy");
                        result.Data.License = license;
                        result.Data.Phone = phone;
                        result.Data.LegacyRefNo = legacyRefNo;
                        result.Data.ApprovedComment = approvedComment;
                        if (!string.IsNullOrEmpty(acctTideInfoModel.LL_Name)) result.Data.CustomerName = acctTideInfoModel.LL_Name;
                        else result.Data.CustomerName = acctTideInfoModel.F_Name + " " + acctTideInfoModel.M_Name + " " + acctTideInfoModel.L_Name;

                        var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.GET_LIST_PROMO_VALID_BY_FILTER");
                        var dyParam = new OracleDynamicParameters();
                        var varDate = Convert.ToDateTime(acctTideInfoModel.Val_Dt);
                        dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam.Add("P_VAL_DT", varDate, OracleMappingType.Date, ParameterDirection.Input);
                        dyParam.Add("P_PROD_CD", acctTideInfoModel.Prod_CD, OracleMappingType.Varchar2, ParameterDirection.Input);
                        dyParam.Add("P_POS_CD", acctTideInfoModel.Pos_CD, OracleMappingType.Varchar2, ParameterDirection.Input);

                        var listProductInstance = await _prmProductInstanceHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<PrmProductInstanceModel>>;
                        if (listProductInstance == null || listProductInstance.StatusCode == StatusCode.Fail) return new ResponseObject<PrmPromotionViewModel>(null, "Không có chương trình khuyến mại phù hợp với sổ tiết kiệm này", StatusCode.Fail);

                        // 2. Theo điều kiện về hạn mức và kỳ hạn mở sổ, theo khách hàng đích danh hoặc khách hàng mới
                        prmProductInstanceHandler = new PrmProductInstanceHandler();
                        var prmTransactionConditionHandler = new PrmTransactionConditionHandler();
                        if (listProductInstance.Data.Count > 0)
                        {
                            foreach (var productInstance in listProductInstance.Data)
                            {
                                var conditionValidResult = new List<PrmTransactionConditionModel>();
                                var isValid = true;
                                var getProductInstanceFullById = await prmProductInstanceHandler.GetByIdAsync(productInstance.Id) as ResponseObject<PrmProductInstanceModel>;
                                if (getProductInstanceFullById.StatusCode == StatusCode.Success && getProductInstanceFullById.Data != null && getProductInstanceFullById.Data.PromotionForm == "Quà tặng")
                                {
                                    if (getProductInstanceFullById.Data.ListTransactionCondition != null && getProductInstanceFullById.Data.ListTransactionCondition.Count > 0)
                                    {
                                        conditionValidResult = getProductInstanceFullById.Data.ListTransactionCondition.Where(sp1 => sp1.FromAmount <= acctTideInfoModel.Avail_Bal
                                                                   && sp1.ToAmount >= acctTideInfoModel.Avail_Bal
                                                                   && sp1.FromTerm <= acctTideInfoModel.Tenure
                                                                   && sp1.ToTerm > acctTideInfoModel.Tenure).ToList();
                                        if (conditionValidResult == null || (conditionValidResult != null && conditionValidResult.Count == 0)) isValid = false;
                                    }
                                }
                                // Khách hàng đích danh
                                if (getProductInstanceFullById.StatusCode == StatusCode.Success && getProductInstanceFullById.Data != null)
                                {
                                    // 2.1. Khách hàng đích đanh
                                    if (getProductInstanceFullById.Data.IsNewCustomer == 0 && getProductInstanceFullById.Data.IdentityCustomerUpload > 0)
                                    {
                                        var listPrmCustomerModel = new List<PrmCustomerModel>();
                                        using (var client = new HttpClient())
                                        {
                                            var requestPrmUploadApiUri = _urlUtilApiEndpoint + "api/v1/prm-customer/filter?query={'UploadTransactionId':" + getProductInstanceFullById.Data.IdentityCustomerUpload + ",'PageIndex':1,'PageSize':100000}";
                                            client.DefaultRequestHeaders.Clear();
                                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                                            var task = await client.GetAsync(requestPrmUploadApiUri);
                                            var jsonString = task.Content.ReadAsStringAsync();
                                            ResponsePrmCustomerModel responsePrmCustomerModel = JsonConvert.DeserializeObject<ResponsePrmCustomerModel>(jsonString.Result);
                                            if (responsePrmCustomerModel.StatusCode == StatusCode.Success)
                                            {
                                                listPrmCustomerModel = responsePrmCustomerModel.Data;
                                                if (!listPrmCustomerModel.Exists(sp => sp.CIFNUMBER.Equals(acctTideInfoModel.Cif_No))) isValid = false;
                                            }
                                            else isValid = false;
                                        }
                                    }
                                    // 2.2 Khách hàng hiện hữu
                                    else if (getProductInstanceFullById.Data.IsNewCustomer == -1)
                                    {
                                        if (acctTideInfoModel.Acct_Status != "A")
                                        {
                                            isValid = false;
                                        }
                                    }
                                    // 2.3 Khách hàng mới
                                    else if (getProductInstanceFullById.Data.IsNewCustomer == 1)
                                    {
                                        var promoStartDate = getProductInstanceFullById.Data.PromotionInfo.StartDate;
                                        var promoEndDate = getProductInstanceFullById.Data.PromotionInfo.EndDate;

                                        var checkNewCus = await CheckIsNewCustomer(acctTideInfoModel.Cif_No, promoStartDate, promoEndDate);
                                        isValid = checkNewCus;
                                    }
                                }
                                // Danh sách hình thức tặng quà
                                if (!string.IsNullOrEmpty(getProductInstanceFullById.Data.SpendForm)) getProductInstanceFullById.Data.ListSpendForm = getProductInstanceFullById.Data.SpendForm.Split(",").ToList();
                                // Tính giá trị quà tặng ( tiền mặt )
                                decimal giaTriTienMat = 0;
                                decimal giaTriDaNhan = 0;
                                if (conditionValidResult != null && conditionValidResult.Count > 0)
                                {
                                    if (conditionValidResult != null && conditionValidResult.Count > 0)
                                    {
                                        var giaTriPhanTram = conditionValidResult.FirstOrDefault().PercentValue;
                                        if (getProductInstanceFullById.Data.GiftValue == "TIEN_MAT")
                                            giaTriTienMat = conditionValidResult.FirstOrDefault().CashValue;
                                        else if (getProductInstanceFullById.Data.GiftValue == "PHAN_TRAM_LAI_SUAT")
                                        {
                                            giaTriTienMat = Math.Round((acctTideInfoModel.Avail_Bal * giaTriPhanTram * acctTideInfoModel.Tenure) / (12 * 100), 0);
                                        }
                                        else if (getProductInstanceFullById.Data.GiftValue == "PHAN_TRAM_TIEN_GUI")
                                        {
                                            giaTriTienMat = acctTideInfoModel.Avail_Bal * giaTriPhanTram / 100;
                                        }
                                    }
                                }
                                getProductInstanceFullById.Data.GiftCashValue = giaTriTienMat;
                                // Tính giá trị đã nhận theo sổ tiết kiệm dựa trên giao dịch đã phê duyệt
                                var getListTransDetailReceivedGift = await GetListTransDetailReceivedGift(legacyRefNo, getProductInstanceFullById.Data.Id) as ResponseObject<List<PrmTransactionLogDetailModel>>;
                                if (getListTransDetailReceivedGift != null && getListTransDetailReceivedGift.StatusCode == StatusCode.Success && getListTransDetailReceivedGift.Data != null)
                                {
                                    giaTriDaNhan = getListTransDetailReceivedGift.Data.Where(sp => sp.Status == "APPROVED").Sum(sp => sp.Quantity * sp.GiftValue);
                                }
                                getProductInstanceFullById.Data.GiftCashValueReceived = giaTriDaNhan;
                                getProductInstanceFullById.Data.ListValidGift = getProductInstanceFullById.Data.ListGift.Where(sp => sp.Price <= (giaTriTienMat - giaTriDaNhan)).ToList();

                                if (isValid) lstPrmProductInstanceValid.Add(getProductInstanceFullById.Data);
                            }
                            result.Data.ListValidProduct = lstPrmProductInstanceValid;
                        }
                    }
                    if (result.Data.ListValidProduct != null && result.Data.ListValidProduct.Count > 0)
                    {
                        // Nếu truyền thêm TransLogId thì lấy TransLogDetail tồn tại để gắn vào danh sách quà đã chọn
                        // sử dụng khi thực hiện sửa giao dịch bị từ chối và sửa để gửi duyệt lại
                        var procName1 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_DETAIL_BY_TRANSACTION");
                        var dyParam1 = new OracleDynamicParameters();
                        dyParam1.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                        dyParam1.Add("P_TRANSACTIONID", transLogId, OracleMappingType.Decimal, ParameterDirection.Input);

                        var getDetailByTransactionId = await _prmTransactionLogDetailHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<PrmTransactionLogDetailModel>>;
                        if (getDetailByTransactionId != null && getDetailByTransactionId.StatusCode == StatusCode.Success)
                        {
                            foreach (var product in result.Data.ListValidProduct)
                            {
                                product.ListPrmTransactionLogDetailModel = new List<PrmTransactionLogDetailModel>();
                                if (getDetailByTransactionId.Data.Where(sp => sp.ProductInstanceId == product.Id) != null)
                                    product.ListPrmTransactionLogDetailModel.AddRange(getDetailByTransactionId.Data.Where(sp => sp.ProductInstanceId == product.Id).ToList());
                            }
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
        public async Task<Response> GetListTransDetailReceivedGift(string legacyRefNo, decimal productInstanceId)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.GET_TRANS_DETAIL_RECEIVED_GIFT");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_LEGACYREFNO", legacyRefNo, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_PRODUCTINSTANCEID", productInstanceId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _prmTransactionLogDetailHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> GetListGiftValidByLegacyRefNo(string legacyRefNo, string license, string phone)
        {
            try
            {
                var result = new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), string.Empty, StatusCode.Success);
                // Lấy thông tin sổ tiết kiệm theo số sổ tk và cmnd
                string requestCoreApiUri = _urlCoreApiEndpoint + "api/core/common/get-acct-tide?legacyRefNo=" + legacyRefNo + "&lincese=" + license + "";
                _logger.LogInformation("GetListGiftValidByLegacyRefNo: Start");
                AcctTideInfoModel acctTideInfoModel = new AcctTideInfoModel();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                    var task = await client.GetAsync(requestCoreApiUri);
                    var jsonString = task.Content.ReadAsStringAsync();
                    ResponseAcctTideInfoModel responseAcctTideModel = JsonConvert.DeserializeObject<ResponseAcctTideInfoModel>(jsonString.Result);
                    _logger.LogInformation("GetListGiftValidByLegacyRefNo: jsonString.Result" + jsonString.Result);
                    if (responseAcctTideModel.StatusCode == StatusCode.Success && responseAcctTideModel.Data != null && responseAcctTideModel.Data.Status != "C") acctTideInfoModel = responseAcctTideModel.Data;
                    else return new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), "Thông tin sổ tiết kiệm không tồn tại hoặc đã tất toán", StatusCode.Fail);
                }
                _logger.LogInformation("GetListGiftValidByLegacyRefNo: jsonString.Result end");
                // Lấy các sản phẩm thuộc chương trình khuyến mãi có điều kiện phù hợp
                // 1. Theo thời gian mở sổ, theo mã sản phẩm, theo chi nhánh mở sổ
                var lstPrmProductInstanceValid = new List<PrmProductInstanceModel>();
                if (acctTideInfoModel != null)
                {
                    if (!acctTideInfoModel.Val_Dt.HasValue || string.IsNullOrEmpty(acctTideInfoModel.Prod_CD)
                        || string.IsNullOrEmpty(acctTideInfoModel.Pos_CD))
                        return new ResponseObject<PrmPromotionViewModel>(null, "Không lấy được thông tin ngày mở sổ, mã sản phẩm, chi nhánh mở sổ", StatusCode.Fail);
                    // Gán thông tin sổ tiết kiệm
                    result.Data.Branch = acctTideInfoModel.Pos_CD;
                    result.Data.CifNo = acctTideInfoModel.Cif_No;
                    result.Data.DepositNo = acctTideInfoModel.Deposit_No;
                    result.Data.LegacyRefNo = acctTideInfoModel.Acct_No;
                    result.Data.DepositVal = acctTideInfoModel.Avail_Bal;
                    result.Data.Tenure = acctTideInfoModel.Tenure;
                    result.Data.ValidDate = acctTideInfoModel.Val_Dt.Value.ToString("dd/MM/yyyy");
                    result.Data.DueDate = acctTideInfoModel.Mat_Dt.Value.ToString("dd/MM/yyyy");
                    result.Data.License = license;
                    result.Data.Phone = phone;
                    if (!string.IsNullOrEmpty(acctTideInfoModel.LL_Name)) result.Data.CustomerName = acctTideInfoModel.LL_Name;
                    else result.Data.CustomerName = acctTideInfoModel.F_Name + " " + acctTideInfoModel.M_Name + " " + acctTideInfoModel.L_Name;

                    _logger.LogInformation("GetListGiftValidByLegacyRefNo: result.Data.CustomerName");
                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.GET_LIST_PROMO_VALID_BY_FILTER");
                    var dyParam = new OracleDynamicParameters();
                    var varDate = Convert.ToDateTime(acctTideInfoModel.Val_Dt);
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_VAL_DT", varDate, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_PROD_CD", acctTideInfoModel.Prod_CD, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_POS_CD", acctTideInfoModel.Pos_CD, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var listProductInstance = await _prmProductInstanceHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<PrmProductInstanceModel>>;
                    if (listProductInstance == null || listProductInstance.StatusCode == StatusCode.Fail) return new ResponseObject<PrmPromotionViewModel>(null, "Không có chương trình khuyến mại phù hợp với sổ tiết kiệm này", StatusCode.Fail);
                    _logger.LogInformation("GetListGiftValidByLegacyRefNo: listProductInstance");
                    // 2. Theo điều kiện về hạn mức và kỳ hạn mở sổ, theo loại khách hàng đích danh, khách hàng hiện hữu hoặc khách hàng mới
                    prmProductInstanceHandler = new PrmProductInstanceHandler();
                    var prmTransactionConditionHandler = new PrmTransactionConditionHandler();
                    if (listProductInstance.Data.Count > 0)
                    {
                        foreach (var productInstance in listProductInstance.Data)
                        {
                            var conditionValidResult = new List<PrmTransactionConditionModel>();
                            var isValid = true;
                            var getProductInstanceFullById = await prmProductInstanceHandler.GetByIdAsync(productInstance.Id) as ResponseObject<PrmProductInstanceModel>;
                            _logger.LogInformation("GetListGiftValidByLegacyRefNo: getProductInstanceFullById");
                            if (getProductInstanceFullById.StatusCode == StatusCode.Success && getProductInstanceFullById.Data != null && getProductInstanceFullById.Data.PromotionForm == "Quà tặng")
                            {
                                if (getProductInstanceFullById.Data.ListTransactionCondition != null && getProductInstanceFullById.Data.ListTransactionCondition.Count > 0)
                                {
                                    conditionValidResult = getProductInstanceFullById.Data.ListTransactionCondition.Where(sp1 => sp1.FromAmount <= acctTideInfoModel.Avail_Bal
                                                               && sp1.ToAmount >= acctTideInfoModel.Avail_Bal
                                                               && sp1.FromTerm <= acctTideInfoModel.Tenure
                                                               && sp1.ToTerm >= acctTideInfoModel.Tenure).ToList();
                                    if (conditionValidResult == null || (conditionValidResult != null && conditionValidResult.Count == 0)) isValid = false;
                                }
                            }
                            // Kiểm tra theo loại khách hàng
                            if (getProductInstanceFullById.StatusCode == StatusCode.Success && getProductInstanceFullById.Data != null)
                            {
                                _logger.LogInformation("GetListGiftValidByLegacyRefNo: getProductInstanceFullById.Data.IsNewCustomer: " + getProductInstanceFullById.Data.IsNewCustomer);
                                // 2.1. Khách hàng đích đanh
                                if (getProductInstanceFullById.Data.IsNewCustomer == 0 && getProductInstanceFullById.Data.IdentityCustomerUpload > 0)
                                {
                                    var listPrmCustomerModel = new List<PrmCustomerModel>();
                                    using (var client = new HttpClient())
                                    {
                                        var requestPrmUploadApiUri = _urlUtilApiEndpoint + "api/v1/prm-customer/filter?query={'UploadTransactionId':" + getProductInstanceFullById.Data.IdentityCustomerUpload + ",'PageIndex':1,'PageSize':100000}";
                                        client.DefaultRequestHeaders.Clear();
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                                        var task = await client.GetAsync(requestPrmUploadApiUri);
                                        var jsonString = task.Content.ReadAsStringAsync();
                                        ResponsePrmCustomerModel responsePrmCustomerModel = JsonConvert.DeserializeObject<ResponsePrmCustomerModel>(jsonString.Result);
                                        if (responsePrmCustomerModel.StatusCode == StatusCode.Success)
                                        {
                                            listPrmCustomerModel = responsePrmCustomerModel.Data;
                                            if (!listPrmCustomerModel.Exists(sp => sp.CIFNUMBER.Equals(acctTideInfoModel.Cif_No))) isValid = false;
                                        }
                                        else isValid = false;
                                    }
                                }
                                // 2.2 Khách hàng hiện hữu
                                else if (getProductInstanceFullById.Data.IsNewCustomer == -1)
                                {
                                    if (acctTideInfoModel.Acct_Status != "A")
                                    {
                                        isValid = false;
                                    }
                                }
                                // 2.3 Khách hàng mới
                                else if (getProductInstanceFullById.Data.IsNewCustomer == 1)
                                {
                                    var promoStartDate = getProductInstanceFullById.Data.PromotionInfo.StartDate;
                                    var promoEndDate = getProductInstanceFullById.Data.PromotionInfo.EndDate;

                                    var checkNewCus = await CheckIsNewCustomer(acctTideInfoModel.Cif_No, promoStartDate, promoEndDate);
                                    isValid = checkNewCus;
                                }
                                // 2.4 Giới tính khách hàng
                                if (!string.IsNullOrEmpty(getProductInstanceFullById.Data.Gender) && getProductInstanceFullById.Data.Gender != "0")
                                    isValid = acctTideInfoModel.Sex_Cd.Equals(getProductInstanceFullById.Data.Gender);

                                // 2.5 Hạng khách hàng
                                if (!string.IsNullOrEmpty(getProductInstanceFullById.Data.CustClass))
                                {
                                    var listCustClassCondition = getProductInstanceFullById.Data.CustClass.Split(',');
                                    // Lấy thông tin hạng khách hàng theo cif
                                    string requestCoreApiUri1 = _urlCoreApiEndpoint + "api/core/common/get-cust-class/by-cif-no?cifNo=" + acctTideInfoModel.Cif_No;
                                    using (var client = new HttpClient())
                                    {
                                        client.DefaultRequestHeaders.Clear();
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                                        var task = await client.GetAsync(requestCoreApiUri1);
                                        var jsonString = task.Content.ReadAsStringAsync();
                                        ResponseListCustClassInfoModel responseListCustClassInfoModel = JsonConvert.DeserializeObject<ResponseListCustClassInfoModel>(jsonString.Result);
                                        if (responseListCustClassInfoModel.StatusCode == StatusCode.Success && responseListCustClassInfoModel.Data != null && responseListCustClassInfoModel.Data.Count > 0)
                                        {
                                            var lstCustClassModel = responseListCustClassInfoModel.Data;
                                            isValid = listCustClassCondition.Any(x => lstCustClassModel.Any(y => y.Code == x));
                                        }
                                    }
                                }
                            }
                            // Danh sách hình thức tặng quà
                            if (!string.IsNullOrEmpty(getProductInstanceFullById.Data.SpendForm)) getProductInstanceFullById.Data.ListSpendForm = getProductInstanceFullById.Data.SpendForm.Split(",").ToList();
                            // Tính giá trị quà tặng ( tiền mặt )
                            decimal giaTriTienMat = 0;
                            decimal giaTriDaNhan = 0;
                            if (conditionValidResult != null && conditionValidResult.Count > 0)
                            {
                                if (conditionValidResult != null && conditionValidResult.Count > 0)
                                {
                                    var giaTriPhanTram = conditionValidResult.FirstOrDefault().PercentValue;
                                    if (getProductInstanceFullById.Data.GiftValue == "TIEN_MAT")
                                        giaTriTienMat = conditionValidResult.FirstOrDefault().CashValue;
                                    else if (getProductInstanceFullById.Data.GiftValue == "PHAN_TRAM_LAI_SUAT")
                                    {
                                        giaTriTienMat = Math.Round((acctTideInfoModel.Avail_Bal * giaTriPhanTram * acctTideInfoModel.Tenure) / (12 * 100), 0);
                                    }
                                    else if (getProductInstanceFullById.Data.GiftValue == "PHAN_TRAM_TIEN_GUI")
                                    {
                                        giaTriTienMat = acctTideInfoModel.Avail_Bal * giaTriPhanTram / 100;
                                    }
                                }
                            }
                            _logger.LogInformation("GetListGiftValidByLegacyRefNo: beforeGetListTransDetailReceivedGift");
                            getProductInstanceFullById.Data.GiftCashValue = giaTriTienMat;
                            // Tính giá trị đã nhận theo sổ tiết kiệm dựa trên giao dịch đã phê duyệt
                            var getListTransDetailReceivedGift = await GetListTransDetailReceivedGift(legacyRefNo, getProductInstanceFullById.Data.Id) as ResponseObject<List<PrmTransactionLogDetailModel>>;
                            _logger.LogInformation("GetListGiftValidByLegacyRefNo: getListTransDetailReceivedGift");
                            if (getListTransDetailReceivedGift != null && getListTransDetailReceivedGift.StatusCode == StatusCode.Success && getListTransDetailReceivedGift.Data != null)
                            {
                                giaTriDaNhan = getListTransDetailReceivedGift.Data.Where(sp => sp.Status == "APPROVED").Sum(sp => sp.Quantity * sp.GiftValue);
                            }
                            getProductInstanceFullById.Data.GiftCashValueReceived = giaTriDaNhan;
                            getProductInstanceFullById.Data.ListValidGift = getProductInstanceFullById.Data.ListGift.Where(sp => sp.Price <= (giaTriTienMat - giaTriDaNhan)).ToList();

                            if (isValid) lstPrmProductInstanceValid.Add(getProductInstanceFullById.Data);
                        }
                        _logger.LogInformation("GetListGiftValidByLegacyRefNo: EndLooop");
                        result.Data.ListValidProduct = lstPrmProductInstanceValid;
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
        public async Task<Response> GetListPromoValidByLegacyRefNo(string legacyRefNo, string license, string phone, string employeePosCd)
        {
            try
            {
                _logger.LogInformation("employeePosCd: " + employeePosCd);
                var result = new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), string.Empty, StatusCode.Success);
                result.Data.TaiKhoanTrungGianDVKH = _tKTGDVKH;
                // Kiểm tra sổ đã được tạo giao dịch tặng quà chưa
                var procName0 = string.Join('.', _dBSchemaName, "PKG_PRM_TRANSACTION_LOG.CHECK_DEPOSIT_EXIST");
                var dyParam0 = new OracleDynamicParameters();
                dyParam0.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam0.Add("P_SAVEACCOUNTNUMBER", legacyRefNo, OracleMappingType.Varchar2, ParameterDirection.Input);

                var checkExistDepositNo = await _prmTransactionConditionLogHandler.ExecuteProcOracleReturnRow(procName0, dyParam0, false) as ResponseObject<PrmTransactionLogModel>;
                if (checkExistDepositNo != null && checkExistDepositNo.StatusCode == StatusCode.Success)
                {
                    return new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), "Sổ này đang có giao dịch 'Chờ Phê Duyệt', vui lòng hoàn thành trước khi tạo giao dịch mới!", StatusCode.Fail);
                }
                // Lấy thông tin sổ tiết kiệm theo số sổ tk và cmnd
                string requestCoreApiUri = _urlCoreApiEndpoint + "api/core/common/get-acct-tide?legacyRefNo=" + legacyRefNo + "&lincese=" + license + "";
                AcctTideInfoModel acctTideInfoModel = new AcctTideInfoModel();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                    var task = await client.GetAsync(requestCoreApiUri);
                    var jsonString = task.Content.ReadAsStringAsync();

                    _logger.LogInformation("jsonString: " + jsonString.Result);
                    ResponseAcctTideInfoModel responseAcctTideModel = JsonConvert.DeserializeObject<ResponseAcctTideInfoModel>(jsonString.Result);
                    if (responseAcctTideModel.StatusCode == StatusCode.Success 
                        && responseAcctTideModel.Data != null 
                        && responseAcctTideModel.Data.Status != "C") acctTideInfoModel = responseAcctTideModel.Data;
                    else return new ResponseObject<PrmPromotionViewModel>(new PrmPromotionViewModel(), "Thông tin sổ tiết kiệm không tồn tại hoặc đã tất toán", StatusCode.Fail);
                }
                // Lấy các sản phẩm thuộc chương trình khuyến mãi có điều kiện phù hợp
                // 1. Theo thời gian mở sổ, theo mã sản phẩm, theo chi nhánh mở sổ
                var lstPrmProductInstanceValid = new List<PrmProductInstanceModel>();
                if (acctTideInfoModel != null)
                {
                    _logger.LogInformation("employeePosCd: " + employeePosCd);
                    if (!string.IsNullOrEmpty(employeePosCd) && acctTideInfoModel.Pos_CD != employeePosCd)
                        return new ResponseObject<PrmPromotionViewModel>(null, "Sổ tiết kiệm không được mở tại chi nhánh này", StatusCode.Fail);
                    if (!acctTideInfoModel.Val_Dt.HasValue || string.IsNullOrEmpty(acctTideInfoModel.Prod_CD)
                        || string.IsNullOrEmpty(acctTideInfoModel.Pos_CD))
                        return new ResponseObject<PrmPromotionViewModel>(null, "Không lấy được thông tin ngày mở sổ, mã sản phẩm, chi nhánh mở sổ", StatusCode.Fail);
                    // Gán thông tin sổ tiết kiệm
                    result.Data.Branch = acctTideInfoModel.Pos_CD;
                    result.Data.BranchName = acctTideInfoModel.Pos_Desc;
                    result.Data.CifNo = acctTideInfoModel.Cif_No;
                    result.Data.DepositNo = acctTideInfoModel.Deposit_No;
                    result.Data.DepositVal = acctTideInfoModel.Avail_Bal;
                    result.Data.Tenure = acctTideInfoModel.Tenure;
                    result.Data.ValidDate = acctTideInfoModel.Val_Dt.Value.ToString("dd/MM/yyyy");
                    result.Data.DueDate = acctTideInfoModel.Mat_Dt.Value.ToString("dd/MM/yyyy");
                    result.Data.License = license;
                    result.Data.Phone = phone;
                    if (!string.IsNullOrEmpty(acctTideInfoModel.LL_Name)) result.Data.CustomerName = acctTideInfoModel.LL_Name;
                    else result.Data.CustomerName = acctTideInfoModel.F_Name + " " + acctTideInfoModel.M_Name + " " + acctTideInfoModel.L_Name;

                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.GET_LIST_PROMO_VALID_BY_FILTER");
                    var dyParam = new OracleDynamicParameters();
                    var varDate = Convert.ToDateTime(acctTideInfoModel.Val_Dt);
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_VAL_DT", varDate, OracleMappingType.Date, ParameterDirection.Input);
                    dyParam.Add("P_PROD_CD", acctTideInfoModel.Prod_CD, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_POS_CD", acctTideInfoModel.Pos_CD, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var listProductInstance = await _prmProductInstanceHandler.ExecuteProcOracleReturnRow(procName, dyParam, true) as ResponseObject<List<PrmProductInstanceModel>>;
                    if (listProductInstance == null || listProductInstance.StatusCode == StatusCode.Fail) return new ResponseObject<PrmPromotionViewModel>(null, "Không có chương trình khuyến mại phù hợp với sổ tiết kiệm này", StatusCode.Fail);

                    // 2. Theo điều kiện về hạn mức và kỳ hạn mở sổ, theo khách hàng đích danh hoặc khách hàng mới
                    prmProductInstanceHandler = new PrmProductInstanceHandler();
                    var prmTransactionConditionHandler = new PrmTransactionConditionHandler();
                    if (listProductInstance.Data.Count > 0)
                    {

                        foreach (var productInstance in listProductInstance.Data)
                        {
                            var conditionValidResult = new List<PrmTransactionConditionModel>();
                            var isValid = true;
                            var getProductInstanceFullById = await prmProductInstanceHandler.GetByIdAsync(productInstance.Id) as ResponseObject<PrmProductInstanceModel>;
                            if (getProductInstanceFullById.StatusCode == StatusCode.Success && getProductInstanceFullById.Data != null && getProductInstanceFullById.Data.PromotionForm == "Quà tặng")
                            {
                                if (getProductInstanceFullById.Data.ListTransactionCondition != null && getProductInstanceFullById.Data.ListTransactionCondition.Count > 0)
                                {
                                    conditionValidResult = getProductInstanceFullById.Data.ListTransactionCondition.Where(sp1 => sp1.FromAmount <= acctTideInfoModel.Avail_Bal
                                                               && sp1.ToAmount >= acctTideInfoModel.Avail_Bal
                                                               && sp1.FromTerm <= acctTideInfoModel.Tenure
                                                               && sp1.ToTerm > acctTideInfoModel.Tenure).ToList();
                                    if (conditionValidResult == null || (conditionValidResult != null && conditionValidResult.Count == 0)) isValid = false;
                                }
                            }
                            // Khách hàng đích danh
                            if (getProductInstanceFullById.StatusCode == StatusCode.Success && getProductInstanceFullById.Data != null)
                            {
                                // 2.1. Khách hàng đích đanh
                                if (getProductInstanceFullById.Data.IsNewCustomer == 0 && getProductInstanceFullById.Data.IdentityCustomerUpload > 0)
                                {
                                    var listPrmCustomerModel = new List<PrmCustomerModel>();
                                    using (var client = new HttpClient())
                                    {
                                        var requestPrmUploadApiUri = _urlUtilApiEndpoint + "api/v1/prm-customer/filter?query={'UploadTransactionId':" + getProductInstanceFullById.Data.IdentityCustomerUpload + ",'PageIndex':1,'PageSize':100000}";
                                        client.DefaultRequestHeaders.Clear();
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                                        var task = await client.GetAsync(requestPrmUploadApiUri);
                                        var jsonString = task.Content.ReadAsStringAsync();
                                        ResponsePrmCustomerModel responsePrmCustomerModel = JsonConvert.DeserializeObject<ResponsePrmCustomerModel>(jsonString.Result);
                                        if (responsePrmCustomerModel.StatusCode == StatusCode.Success)
                                        {
                                            listPrmCustomerModel = responsePrmCustomerModel.Data;
                                            if (!listPrmCustomerModel.Exists(sp => sp.CIFNUMBER.Equals(acctTideInfoModel.Cif_No))) isValid = false;
                                        }
                                        else isValid = false;
                                    }
                                }
                                // 2.2 Khách hàng hiện hữu
                                else if (getProductInstanceFullById.Data.IsNewCustomer == -1)
                                {
                                    if (acctTideInfoModel.Acct_Status != "A")
                                    {
                                        isValid = false;
                                    }
                                }
                                // 2.3 Khách hàng mới
                                else if (getProductInstanceFullById.Data.IsNewCustomer == 1)
                                {
                                    var promoStartDate = getProductInstanceFullById.Data.PromotionInfo.StartDate;
                                    var promoEndDate = getProductInstanceFullById.Data.PromotionInfo.EndDate;

                                    var checkNewCus = await CheckIsNewCustomer(acctTideInfoModel.Cif_No, promoStartDate, promoEndDate);
                                    isValid = checkNewCus;
                                }
                                // 2.4 Giới tính khách hàng
                                if (!string.IsNullOrEmpty(getProductInstanceFullById.Data.Gender) && getProductInstanceFullById.Data.Gender != "0" && isValid)
                                    isValid = acctTideInfoModel.Sex_Cd.Equals(getProductInstanceFullById.Data.Gender);

                                // 2.5 Hạng khách hàng
                                if (!string.IsNullOrEmpty(getProductInstanceFullById.Data.CustClass) && isValid)
                                {
                                    var listCustClassCondition = getProductInstanceFullById.Data.CustClass.Split(',');
                                    // Lấy thông tin hạng khách hàng theo cif
                                    string requestCoreApiUri1 = _urlCoreApiEndpoint + "api/core/common/get-cust-class/by-cif-no?cifNo=" + acctTideInfoModel.Cif_No;
                                    using (var client = new HttpClient())
                                    {
                                        client.DefaultRequestHeaders.Clear();
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                                        var task = await client.GetAsync(requestCoreApiUri1);
                                        var jsonString = task.Content.ReadAsStringAsync();
                                        ResponseListCustClassInfoModel responseListCustClassInfoModel = JsonConvert.DeserializeObject<ResponseListCustClassInfoModel>(jsonString.Result);
                                        if (responseListCustClassInfoModel.StatusCode == StatusCode.Success && responseListCustClassInfoModel.Data != null && responseListCustClassInfoModel.Data.Count > 0)
                                        {
                                            var lstCustClassModel = responseListCustClassInfoModel.Data;
                                            isValid = listCustClassCondition.Any(x => lstCustClassModel.Any(y => y.Code == x));
                                        }
                                    }
                                }
                            }
                            // Danh sách hình thức tặng quà
                            if (!string.IsNullOrEmpty(getProductInstanceFullById.Data.SpendForm)) getProductInstanceFullById.Data.ListSpendForm = getProductInstanceFullById.Data.SpendForm.Split(",").ToList();
                            // Tính giá trị quà tặng ( tiền mặt )
                            decimal giaTriTienMat = 0;
                            decimal giaTriDaNhan = 0;
                            if (conditionValidResult != null && conditionValidResult.Count > 0)
                            {
                                var giaTriPhanTram = conditionValidResult.FirstOrDefault().PercentValue;
                                if (getProductInstanceFullById.Data.GiftValue == "TIEN_MAT")
                                    giaTriTienMat = conditionValidResult.FirstOrDefault().CashValue;
                                else if (getProductInstanceFullById.Data.GiftValue == "PHAN_TRAM_LAI_SUAT")
                                {
                                    giaTriTienMat = Math.Round((acctTideInfoModel.Avail_Bal * giaTriPhanTram * acctTideInfoModel.Tenure) / (12 * 100), 0);
                                }
                                else if (getProductInstanceFullById.Data.GiftValue == "PHAN_TRAM_TIEN_GUI")
                                {
                                    giaTriTienMat = acctTideInfoModel.Avail_Bal * giaTriPhanTram / 100;
                                }
                            }
                            getProductInstanceFullById.Data.GiftCashValue = giaTriTienMat;
                            // Tính giá trị đã nhận theo sổ tiết kiệm dựa trên giao dịch đã phê duyệt
                            var getListTransDetailReceivedGift = await GetListTransDetailReceivedGift(legacyRefNo, getProductInstanceFullById.Data.Id) as ResponseObject<List<PrmTransactionLogDetailModel>>;
                            if (getListTransDetailReceivedGift != null && getListTransDetailReceivedGift.StatusCode == StatusCode.Success && getListTransDetailReceivedGift.Data != null)
                            {
                                giaTriDaNhan = getListTransDetailReceivedGift.Data.Where(sp => sp.Status == "APPROVED").Sum(sp => sp.Quantity * sp.GiftValue);
                            }
                            getProductInstanceFullById.Data.GiftCashValueReceived = giaTriDaNhan;
                            getProductInstanceFullById.Data.ListValidGift = getProductInstanceFullById.Data.ListGift.Where(sp => sp.Price <= (giaTriTienMat - giaTriDaNhan)).ToList();

                            if (isValid) lstPrmProductInstanceValid.Add(getProductInstanceFullById.Data);
                        }
                        result.Data.ListValidProduct = lstPrmProductInstanceValid;
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
        private async Task<bool> CheckIsNewCustomer(string cifNo, DateTime? promoStartDate, DateTime? promoEndDate)
        {
            try
            {
                var result = false;
                // Lấy thông tin sổ tiết kiệm theo số CIF
                string requestCoreApiUri = _urlCoreApiEndpoint + "api/core/common/get-acct-tide/by-cif-no?cifNo=" + cifNo;
                List<AcctTideInfoModel> listAcctTideInfoModel = new List<AcctTideInfoModel>();
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                    var task = await client.GetAsync(requestCoreApiUri);
                    var jsonString = task.Content.ReadAsStringAsync();
                    ResponseListAcctTideInfoModel responseListAcctTideModel = JsonConvert.DeserializeObject<ResponseListAcctTideInfoModel>(jsonString.Result);
                    if (responseListAcctTideModel.StatusCode == StatusCode.Success && responseListAcctTideModel.Data != null)
                    {
                        listAcctTideInfoModel = responseListAcctTideModel.Data;
                        //1. Khách hàng có 1 sổ tại thời điểm kiểm tra thì là khách hàng mới
                        if (listAcctTideInfoModel.Count == 1) result = true;
                        //2. Thông tin sổ tất toán cuối cùng phải trước ngày bắt đầu CTKM và trước 1 năm (365 ngày) ngày kết thúc CTKM
                        else if (listAcctTideInfoModel.Count > 1 && promoStartDate != null && promoEndDate != null)
                        {
                            // Lấy thông tin sổ tất toán cuối cùng
                            var lastAcctide = listAcctTideInfoModel.Where(sp => sp.Status == "C").OrderByDescending(sp => sp.Cls_Dt.Value).FirstOrDefault();
                            var compareWithStartDate = DateTime.Compare(new DateTime(lastAcctide.Cls_Dt.Value.Year, lastAcctide.Cls_Dt.Value.Month, lastAcctide.Cls_Dt.Value.Day), new DateTime(promoStartDate.Value.Year, promoStartDate.Value.Month, promoStartDate.Value.Day));
                            var compareWithEndDate = DateTime.Compare(new DateTime(lastAcctide.Cls_Dt.Value.Year, lastAcctide.Cls_Dt.Value.Month, lastAcctide.Cls_Dt.Value.Day).AddMonths(_thoiGianXacDinhKHMoi), new DateTime(promoEndDate.Value.Year, promoEndDate.Value.Month, promoEndDate.Value.Day));
                            if (compareWithStartDate < 1 && compareWithEndDate < 1) result = true;
                        }
                    }
                    // Không có thông tin tức là khách hàng mới
                    else result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return false;
                }
                else return false;
            }
        }
        #endregion
        public async Task<Response> CreateAsync(PrmPromotionCreateModel model)
        {

            try
            {
                if (model.StartDate != null && model.EndDate != null)
                {
                    model.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 1);
                    model.EndDate = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59);
                }
                string code = "CTKM" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.CREATE_RECORD");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_CODE", code, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_NAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STARTDATE", model.StartDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("P_ENDDATE", model.EndDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("P_ISREGISTERIAT", model.IsRegisterIAT, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_LIMITONPROMOTION", model.LimitOnPromotion, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_LIMITONCUSTOMER", model.LimitOnCustomer, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_CREATEDBY", model.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CREATEDBYPOS", model.CreateByPos, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_CREATEDBYDEPT", model.CreateByDept, OracleMappingType.Varchar2, ParameterDirection.Input);
                if(model.CreateByDept.Length > 4)
                {
                    dyParam.Add("P_CREATEDBYROOTDEPT", model.CreateByDept.Substring(0, 4), OracleMappingType.Varchar2, ParameterDirection.Input);
                }
                else
                {
                    dyParam.Add("P_CREATEDBYROOTDEPT", model.CreateByDept, OracleMappingType.Varchar2, ParameterDirection.Input);
                }
                

                var result = await _prmPromotionHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
        public async Task<Response> UpdateAsync(decimal id, PrmPromotionUpdateModel model)
        {

            try
            {
                if (model.StartDate != null && model.EndDate != null)
                {
                    model.StartDate = new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 1);
                    model.EndDate = new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59);
                }
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.UPDATE_RECORD");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_NAME", model.Name, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_DESCRIPTION", model.Description, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_STARTDATE", model.StartDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("P_ENDDATE", model.EndDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("P_STATUS", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_ISREGISTERIAT", model.IsRegisterIAT, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_LIMITONPROMOTION", model.LimitOnPromotion, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_LIMITONCUSTOMER", model.LimitOnCustomer, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_LASTMODIFIEDBY", model.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmPromotionHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.DELETE_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _prmPromotionHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
        public async Task<Response> ApproveByIdAsync(decimal id, string approvedBy)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.APPROVE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_APPROVEDBY", approvedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmPromotionHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
        public async Task<Response> RejectByIdAsync(decimal id, string approvedBy, string approvedComment)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.REJECT");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_APPROVEDBY", approvedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("P_APPROVEDCOMMENT", approvedComment, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmPromotionHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
        public async Task<Response> UpdateStatusAsync(decimal id, string status)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PROMOTION.UPDATE_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("P_STATUS", status, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _prmPromotionHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
