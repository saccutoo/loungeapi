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
using Newtonsoft.Json;
using System.Collections.Generic;

namespace API.BussinessLogic
{
    public class ElgCheckinHandler : IElgCheckinHandler
    {
        private readonly RepositoryHandler<ElgCheckinPeopleGoWith, ElgCheckinPeopleGoWithBaseModel, ElgCheckinPeopleGoWithQueryModel> _elgCheckinPeopleGoWithHandler
               = new RepositoryHandler<ElgCheckinPeopleGoWith, ElgCheckinPeopleGoWithBaseModel, ElgCheckinPeopleGoWithQueryModel>();

        private string _dBSchemaName;
        private readonly ILogger<ElgCheckinHandler> _logger;

        public ElgCheckinHandler(ILogger<ElgCheckinHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        private async Task<Response> CreateAsync(decimal bookingID, ElgCheckinPeopleGoWithModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKIN.PRC_CREATE_PEOPLE_GO_WITH");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", bookingID, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_name", model.CustName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phone_num", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_ticket_num", model.FlightTicketNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_gender", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_is_customer_shb", model.IsCustomerSHB, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_birthday", model.BirthDay, OracleMappingType.Date, ParameterDirection.Input);

                var result = await _elgCheckinPeopleGoWithHandler.ExecuteProcOracle(procName, dyParam);

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
        public async Task<Response> CheckinAsync(ElgCheckinPeopleGoWithCheckinModel model, ELoungeBaseModel baseModel)
        {
            try
            {                
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKIN.PRC_BOOKING_CHECKIN");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", model.BookingId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_elg_cust_id", model.ElgCustId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_id_new", model.CustIdNew, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_map_voucher_id", model.MapVoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_serial_voucher", model.SerialVoucher, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_ticket_num", model.FlightTicketNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_time_from", model.FlightTimeFrom, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_time_to", model.FlightTimeTo, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_location_from", model.FlightLocationFrom, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_location_to", model.FlightLocationTo, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_booking_time", model.BookingTime, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_booking_note", model.BookingNote, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_numof_people_gowith", model.NumOfPeopleGoWith, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_image_url", model.ImageUrl, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_BSVClassName", model.BSVClassName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_BSVCardNum", model.BSVCardNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_BUYMORESLOTQUANTITY", model.BuyMoreSlotQuantity, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_BUYMORESLOTUNITPRICE", model.BuyMoreSlotUnitPrice, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_BUYMORESLOTTOTAL", model.BuyMoreSlotTotal, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCheckinPeopleGoWithHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;                
                if (result != null && result.Data.Status.Equals("00"))
                {
                    await DeleteAsync(model.BookingId);
                    if (model.lstPeopleGoWith != null && model.lstPeopleGoWith.Count > 0)
                    {
                        foreach (var item in model.lstPeopleGoWith)
                        {                            
                            var res = CreateAsync(result.Data.Id, item);
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

        public async Task<Response> GetListCheckInPeopleGoWith(decimal bookingId)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CHECKIN_PEOPLE_GO_WITH.PRC_GET_CHECKIN_PEOPLE_GO_WITH");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", bookingId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCheckinPeopleGoWithHandler.ExecuteProcOracleReturnRow(procName, dyParam) as ResponseObject<List<ElgCheckinPeopleGoWithBaseModel>>;
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

        private async Task<Response> DeleteAsync(decimal bookingId)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CHECKIN_PEOPLE_GO_WITH.PRC_DEL_CHECKIN_PEOPLE_GO_WITH");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", bookingId, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgCheckinPeopleGoWithHandler.ExecuteProcOracle(procName, dyParam);

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

        //vinhtq1 : 13/12/2021: Thêm hàm checkin mới

        public async Task<Response> CheckinNewAsync(ElgCheckinPeopleGoWithCheckinModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKIN.PRC_BOOKING_CHECKIN_NEW");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", model.BookingId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_elg_cust_id", model.ElgCustId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cust_id_new", model.CustIdNew, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_map_voucher_id", model.MapVoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_serial_voucher", model.SerialVoucher, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_ticket_num", model.FlightTicketNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_time_from", model.FlightTimeFrom, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_time_to", model.FlightTimeTo, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_location_from", model.FlightLocationFrom, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_location_to", model.FlightLocationTo, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_booking_time", model.BookingTime, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_booking_note", model.BookingNote, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_numof_people_gowith", model.NumOfPeopleGoWith, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_image_url", model.ImageUrl, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_BSVClassName", model.BSVClassName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_BSVCardNum", model.BSVCardNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_BUYMORESLOTQUANTITY", model.BuyMoreSlotQuantity, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_BUYMORESLOTUNITPRICE", model.BuyMoreSlotUnitPrice, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_BUYMORESLOTTOTAL", model.BuyMoreSlotTotal, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgCheckinPeopleGoWithHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;
                if (result != null && result.Data.Status.Equals("00"))
                {
                    await DeleteAsync(model.BookingId);
                    if (model.lstPeopleGoWith != null && model.lstPeopleGoWith.Count > 0)
                    {
                        foreach (var item in model.lstPeopleGoWith)
                        {
                            var res = CreateAsync(result.Data.Id, item);
                        }
                    }

                    #region Bắn lại thông tin map vs khách hàng cho bên máy nhận diện khuôn mặc PCSB và thêm mới hoặc cập nhật thông tin vào bảng ELG_FACE_CUSTOMER
                    if (!string.IsNullOrEmpty(model.FaceId))
                    {
                        var _elgCustomerHandler = new ElgCustomerHandler();
                        ElgCheckBookingModel modelRes = new ElgCheckBookingModel();
                        // KYC Customer first by query
                        // lay danh sach quyen loi khach hang
                        var respCustomer = await _elgCustomerHandler.GetByIdAsync(model.ElgCustId) as ResponseObject<ElgCustomerBaseModel>;
                        ElgCustomerDecryptModel modelPushToAIPCSB = new ElgCustomerDecryptModel()
                        {
                            Id = model.ElgCustId,
                            FullName = respCustomer.Data.FullName,
                            CustId = respCustomer.Data.CustId,
                            PhoneNum = respCustomer.Data.PhoneNum,
                            RepresentUserName = respCustomer.Data.RepresentUserName,
                            Email = respCustomer.Data.Email,
                            Gender = respCustomer.Data.Gender
                        };

                        var value = JsonConvert.SerializeObject(modelPushToAIPCSB);
                        value = EncryptedString.EncryptString(value, model.FaceId);

                        var _elgNotificationHandler = new ElgNotificationHandler();
                       
                        var cacheValueNoti=await _elgNotificationHandler.GetRedisNotification(model.FaceId);
                        if (cacheValueNoti!=null)
                        {
                            ElgNotificationViewModel modelCacheNotification = new ElgNotificationViewModel()
                            {
                                Id = cacheValueNoti.Id,
                                Value = value,
                                Base64 = cacheValueNoti.Base64,
                                FaceId = model.FaceId,
                                Other = cacheValueNoti.Other
                            };
                            _elgNotificationHandler.SetRedisNotification(model.FaceId, modelCacheNotification);
                        }

                        var responseNotification = await _elgNotificationHandler.GetByFaceIdAsync(model.FaceId) as ResponseObject<List<ElgNotificationViewModel>>;
                        if (responseNotification != null && responseNotification.Data != null)
                        {
                            foreach (var item in responseNotification.Data)
                            {
                                if (string.IsNullOrEmpty(item.Value))
                                {
                                    ElgNotificationUpdateModel updateNoti = new ElgNotificationUpdateModel()
                                    {
                                        Id = item.Id,
                                        FaceId = item.FaceId,
                                        Value = value
                                    };
                                    await _elgNotificationHandler.UpdateAsync(updateNoti);
                                }
                            }
                        }

                        var _elgFaceCustomerHandler = new ElgFaceCustomerHandler();
                        var responseFaceCustomer = await _elgFaceCustomerHandler.GetByFaceIdAsync(model.FaceId) as ResponseObject<ElgFaceCustomerViewModel>;
                        if (responseFaceCustomer != null && responseFaceCustomer.Data != null)
                        {
                            ElgFaceCustomerUpdateModel updateModel = new ElgFaceCustomerUpdateModel()
                            {
                                FaceId = model.FaceId,
                                CustId = respCustomer.Data.CustId,
                                PhoneNum = respCustomer.Data.PhoneNum,
                                UpdatedBy = baseModel.CreateBy
                            };
                            await _elgFaceCustomerHandler.UpdateAsync(updateModel);
                        }
                        else
                        {
                            ElgFaceCustomerCreateModel createModel = new ElgFaceCustomerCreateModel()
                            {
                                FaceId = model.FaceId,
                                CustId = respCustomer.Data.CustId,
                                PhoneNum = respCustomer.Data.PhoneNum,
                                CreateBy = baseModel.CreateBy
                            };
                            await _elgFaceCustomerHandler.CreateAsync(createModel);
                        }


                        //await 
                    }
                    #endregion                  
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
    }
}
