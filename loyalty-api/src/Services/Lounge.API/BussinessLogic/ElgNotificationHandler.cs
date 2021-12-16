using API.Infrastructure.Migrations;
using API.Infrastructure.Repositories;
using API.Interface;
using API.Models;
using Dapper.Oracle;
using Lounge.API;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Utils;
using System.Linq;

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
        private readonly IRedisService _redisService;

        public ElgNotificationHandler(ILogger<ElgNotificationHandler> logger = null, IRedisService redisService = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
            _PackageName = "PKG_ELG_NOTIFICATION";
            _redisService = redisService;
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
                        var cacheValue =await GetRedisNotification(item.FaceId);
                        if (cacheValue!=null)
                        {
                            item.Base64 = cacheValue.Base64;
                        }
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            try
                            {
                                item.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(item.Value, item.FaceId));
                            }
                            catch (Exception)
                            {
                                item.ValueDecrypt = null;
                            }
                        }
                        else
                        {
                            item.ValueDecrypt = null;
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
                    var cacheValue = await GetRedisNotification(result.Data.FaceId);
                    if (cacheValue != null)
                    {
                        result.Data.Base64 = cacheValue.Base64;
                    }
                    if (!string.IsNullOrEmpty(result.Data.Value))
                    {
                        try
                        {
                            result.Data.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(result.Data.Value, result.Data.FaceId));
                        }
                        catch (Exception)
                        {
                            result.Data.ValueDecrypt = null;
                        }
                    }
                    else
                    {
                        result.Data.ValueDecrypt = null;
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
                        var cacheValue = await GetRedisNotification(item.FaceId);
                        if (cacheValue != null)
                        {
                            item.Base64 = cacheValue.Base64;
                        }
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            try
                            {
                                item.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(item.Value, item.FaceId));
                            }
                            catch (Exception)
                            {
                                item.ValueDecrypt = null;
                            }
                        }
                        else
                        {
                            item.ValueDecrypt = null;
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
                        var cacheValue = await GetRedisNotification(item.FaceId);
                        if (cacheValue != null)
                        {
                            item.Base64 = cacheValue.Base64;
                        }
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            try
                            {
                                item.ValueDecrypt = JsonConvert.DeserializeObject<ElgCustomerDecryptModel>(EncryptedString.DecryptString(item.Value, item.FaceId));
                            }
                            catch (Exception)
                            {
                                item.ValueDecrypt = null;
                            }
                        }
                        else
                        {
                            item.ValueDecrypt = null;
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

        public async Task<Response> CreateAsync(ElgNotificationCreateModel model)
        {
            try
            {
                var value = string.Empty;
                var _elgFaceCustomerHandler = new ElgFaceCustomerHandler();
                var responseFaceCustomer = await _elgFaceCustomerHandler.GetByFaceIdAsync(model.FaceId) as ResponseObject<ElgFaceCustomerViewModel>;
                if (responseFaceCustomer != null && responseFaceCustomer.Data != null && !string.IsNullOrEmpty(responseFaceCustomer.Data.CustId))
                {
                    var _elgCustomerHandler = new ElgCustomerHandler();
                    var responseCustomer = await _elgCustomerHandler.GetDistinctByCustIdAsync(responseFaceCustomer.Data.CustId) as ResponseObject<List<ElgCustomerBaseModel>>;
                    if (responseCustomer != null && responseCustomer.Data != null && responseCustomer.Data.Count > 0)
                    {
                        List<ElgCustomerBaseModel> listAprove = responseCustomer.Data.Where(s => s.Status == "APPROVED").ToList();
                        if (listAprove != null && listAprove.Count > 0)
                        {
                            var data = listAprove.LastOrDefault();
                            ElgCustomerDecryptModel modelPushToAIPCSB = new ElgCustomerDecryptModel()
                            {
                                FullName = data.FullName,
                                CustId = data.CustId,
                                PhoneNum = data.PhoneNum,
                                RepresentUserName = data.RepresentUserName,
                                Email = data.Email,
                                Gender = data.Gender
                            };

                            value = JsonConvert.SerializeObject(modelPushToAIPCSB);
                            value = EncryptedString.EncryptString(value, model.FaceId);
                        }
                        else
                        {
                            List<ElgCustomerBaseModel> listExpire = responseCustomer.Data.Where(s => s.Status == "EXPIRED").ToList();
                            if (listExpire != null && listExpire.Count > 0)
                            {
                                var data = listExpire.LastOrDefault();
                                ElgCustomerDecryptModel modelPushToAIPCSB = new ElgCustomerDecryptModel()
                                {
                                    FullName = data.FullName,
                                    CustId = data.CustId,
                                    PhoneNum = data.PhoneNum,
                                    RepresentUserName = data.RepresentUserName,
                                    Email = data.Email,
                                    Gender = data.Gender
                                };

                                value = JsonConvert.SerializeObject(modelPushToAIPCSB);
                                value = EncryptedString.EncryptString(value, model.FaceId);
                            }

                        }

                    }
                }

                var procName = _dBSchemaName + "." + _PackageName + "." + "INSERT_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", value, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PFACEID", model.FaceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PVALUE", value, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PCREATEBY", model.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _ElgNotificationHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result != null && result?.Data != null && result.Data.Status == "00")
                {
                    
                    ElgNotificationViewModel modelCacheTemplate = new ElgNotificationViewModel()
                    {
                        Id = result.Data.Id,
                        Value = value,
                        Base64 = model.base64,
                        FaceId = model.FaceId,
                        Other = model.Other
                    };                   
                    SetRedisNotification(model.FaceId, modelCacheTemplate);
                }
                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return new ResponseError(StatusCode.Fail, ex.Message);
                }
                else throw ex;
            }
        }

        public async Task<Response> UpdateAsync(ElgNotificationUpdateModel model)
        {
            try
            {

                var procName = _dBSchemaName + "." + _PackageName + "." + "INSERT_ROW";
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("PID", model.Id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("PFACEID", model.FaceId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("PVALUE", model.Value, OracleMappingType.Varchar2, ParameterDirection.Input);
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

        #region Redis
        public async void SetRedisNotification(string faceId, ElgNotificationViewModel modelCacheTemplate)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(Convert.ToInt32(Helpers.GetConfig("Redis:Expired"))));

            //set cache lên redis với bảng template

            var cacheKey = Helpers.GetConfig("Redis:Key") + Helpers.GetConfig("Redis:ElgNotification") + faceId.Trim();
            //remove cache cũ
            await _redisService.ClearCache(cacheKey);

            // cachedTime = "Expired";
            // Nạp  giá trị mới cho cache
            await _redisService.SetCache<ElgNotificationViewModel>(cacheKey, modelCacheTemplate, options);
        }

        public async Task<ElgNotificationViewModel> GetRedisNotification(string faceId)
        {
            ElgNotificationViewModel valueReturn = null;
            var cacheKey = Helpers.GetConfig("Redis:Key") + Helpers.GetConfig("Redis:ElgNotification") + faceId.Trim();
            var cachedValue = await _redisService.GetFromCache<ElgNotificationViewModel>(cacheKey);
            if (cachedValue != null)
            {
                valueReturn = cachedValue;
            }
            return valueReturn;
        }
        #endregion
    }
}
