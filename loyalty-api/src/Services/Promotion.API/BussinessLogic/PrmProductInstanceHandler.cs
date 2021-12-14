using API.Infrastructure.Migrations;
using API.Infrastructure.Repositories;
using API.Interface;
using API.Models;
using Dapper.Oracle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class PrmProductInstanceHandler : IPrmProductInstanceHandler
    {
        private readonly RepositoryHandler<PrmProductInstance, PrmProductInstanceModel, PrmProductInstanceQueryModel> _prmProductInstanceHandler
               = new RepositoryHandler<PrmProductInstance, PrmProductInstanceModel, PrmProductInstanceQueryModel>();
        private PrmTransactionConditionHandler _prmTransactionConditionHandler;
        private PrmGiftHandler _prmGiftHandler;
        private PrmOtherConditionHandler _prmOtherConditionHandler;
        private PrmPromotionHandler _prmPromotionHandler;
        private readonly string _dBSchemaName;
        private readonly ILogger<PrmProductInstanceHandler> _logger;

        public PrmProductInstanceHandler(ILogger<PrmProductInstanceHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        public async Task<Response> GetByIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT_INSTANCE.GET_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                // Lây thông tin chung của sản phẩm
                var getById = await _prmProductInstanceHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<PrmProductInstanceModel>;
                if (getById.StatusCode == StatusCode.Success)
                {
                    // Lấy thông tin danh sách điều kiện giao dịch
                    _prmTransactionConditionHandler = new PrmTransactionConditionHandler();
                    var getTransCondition = await _prmTransactionConditionHandler.GetByProductionInstanceIdAsync(id) as ResponseObject<List<PrmTransactionConditionModel>>;
                    if (getTransCondition != null && getTransCondition.StatusCode == StatusCode.Success) getById.Data.ListTransactionCondition = getTransCondition.Data;

                    // Lấy thông tin danh sách điều kiện khác
                    _prmOtherConditionHandler = new PrmOtherConditionHandler();
                    var getOtherCondition = await _prmOtherConditionHandler.GetByProductionInstanceIdAsync(id) as ResponseObject<List<PrmOtherConditionModel>>;
                    if (getOtherCondition != null && getOtherCondition.StatusCode == StatusCode.Success) getById.Data.ListOtherCondition = getOtherCondition.Data;

                    // Lấy thông tin danh sách quà tặng
                    _prmGiftHandler = new PrmGiftHandler();
                    var getGift = await _prmGiftHandler.GetByProductionInstanceIdAsync(id) as ResponseObject<List<PrmGiftModel>>;
                    if (getGift != null && getGift.StatusCode == StatusCode.Success) getById.Data.ListGift = getGift.Data;

                    // Lấy thông tin CTKM
                    _prmPromotionHandler = new PrmPromotionHandler();
                    var getPromo = await _prmPromotionHandler.GetByIdAsync(getById.Data.PromotionId) as ResponseObject<PrmPromotionModel>;
                    if (getPromo != null && getPromo.StatusCode == StatusCode.Success) getById.Data.PromotionInfo = getPromo.Data;
                }
                return getById;
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
        public async Task<Response> GetByPromotionIdAsync(decimal promoId)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT_INSTANCE.GET_BY_PROMOTION_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_PROMOTION_ID", promoId, OracleMappingType.Decimal, ParameterDirection.Input);

                return await _prmProductInstanceHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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
        public async Task<Response> CreateAsync(PrmProductInstanceCreateModel model)
        {
            using (var unitOfWork = new UnitOfWorkOracle())
            {
                var iConn = unitOfWork.GetConnection();
                var iTrans = iConn.BeginTransaction();
                try
                {
                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT_INSTANCE.CREATE_RECORD");
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_PROMOTIONID", model.PromotionId, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_PRODUCTID", model.ProductId, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_PROMOTIONFORM", model.PromotionForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_LIMITONCUSTOMER", model.LimitOnCustomer, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_GIFTVALUE", model.GiftValue, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_SPENDFORM", model.SpendForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_BRANCHSCOPE", model.BranchScope, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_ISNEWCUSTOMER", model.IsNewCustomer, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_IDENTITYCUSTOMERUPLOAD", model.IdentityCustomerUpload, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_NOTIFICATIONCHANNEL", model.NotificationChannel, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_NOTIFICATIONIMAGE", model.NotificationImage, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_NOTIFICATIONLINK", model.NotificationLink, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_CREATEDBY", model.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_PRODUCTCOPYID", model.ProductCopyId, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_GENDER", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_CUSTCLASS", model.CustClass, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var result = await _prmProductInstanceHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                    if (result.StatusCode == StatusCode.Success)
                    {
                        var prodInsId = result.Data.Id;
                        // Thêm danh sách điều kiện giao dịch
                        if (model.ListTransactionCondition != null && model.ListTransactionCondition.Count > 0)
                        {
                            _prmTransactionConditionHandler = new PrmTransactionConditionHandler();
                            var creatTransactionCondition = await _prmTransactionConditionHandler.CreateMultiAsync(iConn, iTrans, prodInsId, model.ListTransactionCondition);
                            if (creatTransactionCondition.StatusCode == StatusCode.Fail)
                            {
                                iTrans.Rollback();
                                return new ResponseError(StatusCode.Fail, "Insert Transaction Condition Fail");
                            }
                        }

                        // Thêm danh sách quà tặng
                        if (model.ListGift != null && model.ListGift.Count > 0)
                        {
                            _prmGiftHandler = new PrmGiftHandler();
                            var creatTransactionCondition = await _prmGiftHandler.CreateMultiAsync(iConn, iTrans, prodInsId, model.ListGift);
                            if (creatTransactionCondition.StatusCode == StatusCode.Fail)
                            {
                                iTrans.Rollback();
                                return new ResponseError(StatusCode.Fail, "Insert Gift Fail");
                            }
                        }

                        // Thêm danh sách điều kiện khác
                        if (model.ListOtherCondition != null && model.ListOtherCondition.Count > 0)
                        {
                            _prmOtherConditionHandler = new PrmOtherConditionHandler();
                            var creatTransactionCondition = await _prmOtherConditionHandler.CreateMultiAsync(iConn, iTrans, prodInsId, model.ListOtherCondition);
                            if (creatTransactionCondition.StatusCode == StatusCode.Fail)
                            {
                                iTrans.Rollback();
                                return new ResponseError(StatusCode.Fail, "Insert Gift Fail");
                            }
                        }
                        iTrans.Commit();
                    }
                    return result;
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
        public async Task<Response> UpdateAsync(decimal id, PrmProductInstanceUpdateModel model)
        {
            using (var unitOfWork = new UnitOfWorkOracle())
            {
                var iConn = unitOfWork.GetConnection();
                var iTrans = iConn.BeginTransaction();
                try
                {
                    var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT_INSTANCE.UPDATE_RECORD");
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                    dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_PROMOTIONFORM", model.PromotionForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_LIMITONCUSTOMER", model.LimitOnCustomer, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_GIFTVALUE", model.GiftValue, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_SPENDFORM", model.SpendForm, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_BRANCHSCOPE", model.BranchScope, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_ISNEWCUSTOMER", model.IsNewCustomer, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_IDENTITYCUSTOMERUPLOAD", model.IdentityCustomerUpload, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_NOTIFICATIONCHANNEL", model.NotificationChannel, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_NOTIFICATIONIMAGE", model.NotificationImage, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_NOTIFICATIONLINK", model.NotificationLink, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_LASTMODIFIEDBY", model.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_PRODUCTCOPYID", model.ProductCopyId, OracleMappingType.Decimal, ParameterDirection.Input);
                    dyParam.Add("P_GENDER", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                    dyParam.Add("P_CUSTCLASS", model.CustClass, OracleMappingType.Varchar2, ParameterDirection.Input);

                    var result = await _prmProductInstanceHandler.ExecuteProcOracle(procName, iConn, iTrans, dyParam) as ResponseObject<ResponseModel>;
                    if (result.StatusCode == StatusCode.Success)
                    {
                        var prodInsId = result.Data.Id;

                        #region Xóa dữ liệu cũ
                        _prmTransactionConditionHandler = new PrmTransactionConditionHandler();
                        _prmGiftHandler = new PrmGiftHandler();
                        _prmOtherConditionHandler = new PrmOtherConditionHandler();

                        var deleteTransactionCondition = await _prmTransactionConditionHandler.DeleteByProductionInstanceIdAsync(prodInsId);
                        var deleteGift = await _prmGiftHandler.DeleteByProductionInstanceIdAsync(prodInsId);
                        var deleteOtherCondition = await _prmOtherConditionHandler.DeleteByProductionInstanceIdAsync(prodInsId);
                        #endregion

                        // Thêm danh sách điều kiện giao dịch
                        if (model.ListTransactionCondition != null && model.ListTransactionCondition.Count > 0 && deleteTransactionCondition.StatusCode == StatusCode.Success)
                        {
                            
                            var creatTransactionCondition = await _prmTransactionConditionHandler.CreateMultiAsync(iConn, iTrans, prodInsId, model.ListTransactionCondition);
                            if (creatTransactionCondition.StatusCode == StatusCode.Fail)
                            {
                                iTrans.Rollback();
                                return new ResponseError(StatusCode.Fail, "Insert Transaction Condition Fail");
                            }
                        }

                        // Thêm danh sách quà tặng
                        if (model.ListGift != null && model.ListGift.Count > 0 && deleteGift.StatusCode == StatusCode.Success)
                        {
                            
                            var creatTransactionCondition = await _prmGiftHandler.CreateMultiAsync(iConn, iTrans, prodInsId, model.ListGift);
                            if (creatTransactionCondition.StatusCode == StatusCode.Fail)
                            {
                                iTrans.Rollback();
                                return new ResponseError(StatusCode.Fail, "Insert Gift Fail");
                            }
                        }

                        // Thêm danh sách điều kiện khác
                        if (model.ListOtherCondition != null && model.ListOtherCondition.Count > 0 && deleteOtherCondition.StatusCode == StatusCode.Success)
                        {
                            
                            var creatTransactionCondition = await _prmOtherConditionHandler.CreateMultiAsync(iConn, iTrans, prodInsId, model.ListOtherCondition);
                            if (creatTransactionCondition.StatusCode == StatusCode.Fail)
                            {
                                iTrans.Rollback();
                                return new ResponseError(StatusCode.Fail, "Insert Gift Fail");
                            }
                        }
                        iTrans.Commit();
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
                    throw ex;
                }
            }
        }
        public async Task<Response> DeleteByIdAsync(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_PRM_PRODUCT_INSTANCE.DELETE_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("P_ID", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _prmProductInstanceHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

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
