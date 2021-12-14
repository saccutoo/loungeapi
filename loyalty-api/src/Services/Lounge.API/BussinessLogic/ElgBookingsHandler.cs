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
    public class ElgBookingsHandler : IElgBookingsHandler
    {
        private readonly RepositoryHandler<ElgBookings, ElgBookingsBaseModel, ElgBookingsQueryModel> _elgBookingsHandler
               = new RepositoryHandler<ElgBookings, ElgBookingsBaseModel, ElgBookingsQueryModel>();

        private readonly RepositoryHandler<ElgBookingCheckout, ElgBookingCheckoutBaseModel,  ElgBookingsQueryModel> _elgBookingFullsHandler
               = new RepositoryHandler<ElgBookingCheckout, ElgBookingCheckoutBaseModel, ElgBookingsQueryModel>();

        private readonly RepositoryHandler<ElgPartnerClass, ElgPartnerClass, ElgCheckinPeopleGoWithQueryModel> _elgPartnerClassHandler
               = new RepositoryHandler<ElgPartnerClass, ElgPartnerClass, ElgCheckinPeopleGoWithQueryModel>();

        private readonly RepositoryHandler<ElgBookingCheckinModel, ElgBookingCheckinModel, ElgCheckInQueryModel> _elgBookingsCheckinHandler
       = new RepositoryHandler<ElgBookingCheckinModel, ElgBookingCheckinModel, ElgCheckInQueryModel>();

        private string _dBSchemaName;

        private readonly ILogger<ElgBookingsHandler> _logger;
        private ElgCustomerHandler _elgCustomerHandler;
        private readonly EmailHandler emailHandler = new EmailHandler();

        private const string ERROR_BOOKING = "Quý Khách không còn lượt sử dụng phòng chờ sân bay/ đã hết hiệu lực/ đã bị hủy. Vui lòng LH Hotline….";
        private const string ERROR_BOOKING_KYC_CUSTOMER = "SĐT / CMTND / Mã đặt chỗ/ Mã voucher không hợp lệ.Vui lòng nhập lại.";
        private const string STATUS_BOOKING = "BOOKING";
        private const string STATUS_CHECKIN = "CHECKIN";
        private const string STATUS_CHECKOUT = "CHECKOUT";
        private const string PARTNER_CODE = "BONG_SEN_VANG";

        public ElgBookingsHandler(ILogger<ElgBookingsHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
        }
        public async Task<Response> CreateAsync(ElgBookingsCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {


                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_CREATE_BOOKING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_elg_cust_id", model.ElgCustId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_map_voucher_id", model.MapVoucherId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_lounge_id", model.LoungeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_booking_time", model.BookingTime, OracleMappingType.Date, ParameterDirection.Input);
                // dyParam.Add("i_reservation_code", model.ReservationCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_booking_note", model.BookingNote, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_numof_people_gowith", model.NumOfPeopleGoWith, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracle(procName, dyParam) as ResponseObject<ResponseModel>;

                if (result.StatusCode == StatusCode.Success && result.Data != null)
                {
                    // Get detail by ID
                    var id = result.Data.Id;
                    var modelDetail = await GetByIdAsync(id) as ResponseObject<ElgBookingsBaseModel>;
                    if (modelDetail.StatusCode == StatusCode.Success && modelDetail.Data != null)
                    {
                        var mailConfirmModel = new ElgEmailBookingModel
                        {
                            Email = modelDetail.Data.Email,
                            ReservationCode = modelDetail.Data.ReservationCode,
                            UserFullName = modelDetail.Data.FullName,
                            PhoneNumber = modelDetail.Data.PhoneNum,
                            PersonGoWith = modelDetail.Data.NumOfPeopleGoWith.ToString(),
                            BookingTime = modelDetail.Data.BookingTime.ToString("HH:mm dd/MM/yyyy")
                        };
                        _ = emailHandler.SendMailSuccessfullyBooking(mailConfirmModel);
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
        public async Task<Response> UpdateAsync(decimal id, ElgBookingsCreateUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_UPDATE_BOOKING");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cust_id", model.CustId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_elg_cust_id", model.ElgCustId, OracleMappingType.Decimal, ParameterDirection.Input);
                //dyParam.Add("i_cust_name", model.CustName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_lounge_id", model.LoungeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_booking_time", model.BookingTime, OracleMappingType.Date, ParameterDirection.Input);
                //dyParam.Add("i_reservation_code", model.ReservationCode, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_booking_note", model.BookingNote, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_numof_people_gowith", model.NumOfPeopleGoWith, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracle(procName, dyParam);
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
        public async Task<Response> GetByFilterAsync(ElgBookingsQueryModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_FILTER_BOOKINGS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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

        public async Task<Response> GetReserverBookingFilterAsync(ElgBookingsQueryModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.GET_FILTER_BOOKING_RESERVATION");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_startdate", model.StartDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_enddate", model.EndDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingFullsHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);


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

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_UPDATE_BOOKING_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("ElgBookings - UpdateStatusAsync - RES:" + JsonConvert.SerializeObject(result));
                if (result.StatusCode == StatusCode.Success && status.Equals("CANCEL"))
                {
                    // Get detail by ID
                    var modelDetail = await GetByIdAsync(id) as ResponseObject<ElgBookingsBaseModel>;
                    if (modelDetail.StatusCode == StatusCode.Success && modelDetail.Data != null)
                    {
                        var mailCancelModel = new ElgEmailBookingModel
                        {
                            Email = modelDetail.Data.Email,
                            UserFullName = modelDetail.Data.FullName,
                            ReservationCode = modelDetail.Data.ReservationCode
                        };
                        _ = emailHandler.SendMailCancelBooking(mailCancelModel);
                        //_ = emailHandler.SendSMSOTPCancelBooking(modelDetail.Data.Email, modelDetail.Data.FullName, modelDetail.Data.PhoneNum, modelDetail.Data.ReservationCode);
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

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_GET_BOOKING_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_booking_id", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);


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
        public async Task<Response> KYCBookingAsync(string query)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_KYC_BOOKINGS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_search_text", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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

        public async Task<Response> KYCBookingV2Async(string query)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_KYC_BOOKINGS_V2");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_list_cust_id", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

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

        public async Task<Response> CheckBookingAsync(string query)
        {
            try
            {
                // Kiểm tra nếu tìm kiếm theo mã đặt chỗ thì mã này đã checkin hoặc checkout chưa? có đúng ngày hay không?
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.GET_BOOKING_INFO_BY_CODE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pReservationcode", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<ElgBookingsBaseModel>;
                decimal elgCustId = 0;
                var custId = "";
                var reservationcode = "";
                var cifNum = "";
                decimal bookingId = 0;
                if (result != null && result.StatusCode == StatusCode.Success && result.Data != null)
                {
                    reservationcode = query;
                    query = string.IsNullOrEmpty(result?.Data?.CustId) ? result.Data.CifNum : result?.Data?.CustId;
                    cifNum = result.Data.CifNum;
                    elgCustId = result.Data.Elg_Cust_Id;
                    bookingId = result.Data.Id.HasValue ? result.Data.Id.Value : 0;
                    custId = result.Data.CustId;
                    var notValidStatus = new List<string>() { "CHECKIN", "CHECKOUT", "CANCEL" };
                    if (notValidStatus.Contains(result.Data.Status)) return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING_KYC_CUSTOMER, StatusCode.Success);
                    // Kiểm tra ngày đăng ký
                    var startToDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
                    var endToDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    var isInTime = (DateTime.Compare(result.Data.BookingTime, startToDay) >= 0 && DateTime.Compare(result.Data.BookingTime, endToDay) <= 0);
                    if (!isInTime) return new ResponseObject<ElgCheckBookingModel>(null, "Mã đặt chỗ đã quá ngày hoặc chưa đến ngày đăng ký", StatusCode.Success);
                }
                
                _elgCustomerHandler = new ElgCustomerHandler();
                ElgCheckBookingModel modelRes = new ElgCheckBookingModel();
                // KYC Customer first by query
                var respCustomerKYC = await _elgCustomerHandler.KYCCustomerAsync(query) as ResponseObject<List<ElgCustomerBaseModel>>;

                if (respCustomerKYC?.Data == null)
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check CustomerKYC: FAIL");

                    return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING_KYC_CUSTOMER, StatusCode.Success);
                }

                modelRes.customerModel = respCustomerKYC.Data.FirstOrDefault();
                // Get cust info
                ElgCustomerInfoHandler _elgCustomerInfoHandler = new ElgCustomerInfoHandler();
                var getInfoById = await _elgCustomerInfoHandler.GetByCustIdAsync(modelRes.customerModel.CustId) as ResponseObject<ElgCustomerInfoModel>;
                if (getInfoById != null) modelRes.customerModel.AllowPrivateRoom = getInfoById.Data.AllowPrivateRoom;
                // Get MaxCheckIn
                decimal MaxCheckIn = modelRes.customerModel.MaxCheckin;
                // KYC Booking by custId
                var responseBookingKYC = await KYCBookingAsync(modelRes.customerModel.CustId) as ResponseObject<List<ElgBookingsBaseModel>>;

                // Lấy danh sách hạng khách hàng theo đối tác Bông Sen Vàng
                var procName1 = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKIN.PRC_GET_LIST_CLASS_BY_PARTNER");
                var dyParam1 = new OracleDynamicParameters();
                dyParam1.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam1.Add("p_Partner_Code", PARTNER_CODE, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result1 = await _elgPartnerClassHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<ElgPartnerClass>>;
                if (result1 != null && result1.StatusCode == StatusCode.Success) modelRes.LstElgPartnerClass = result1.Data;

                if (responseBookingKYC?.Data == null)
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check BookingKYC: FAIL");
                    return new ResponseObject<ElgCheckBookingModel>(modelRes, "Thành công", StatusCode.Success);
                }
                _logger.LogInformation("ElgBookings - CheckBookingAsync - RES:" + JsonConvert.SerializeObject(responseBookingKYC));
                // Step1: Check list booking over maxcheckin or not
                List<ElgBookingsBaseModel> lstCheckedInOrCheckedOut = responseBookingKYC.Data.Where(x => STATUS_CHECKIN.Equals(x.Status) || STATUS_CHECKOUT.Equals(x.Status)).ToList();

                var checkedInByClassId = lstCheckedInOrCheckedOut.GroupBy(x => new { x.ClassId, x.Elg_Cust_Id}).Select(g => new { ClassId = g.Key.ClassId, ElgCustId = g.Key.Elg_Cust_Id, CountUsed = g.Count()}).ToList();
                var isMaxCheckin = true;
                foreach (var item in respCustomerKYC.Data)
                {
                    var isBooking = false;
                    foreach (var itemChecked in checkedInByClassId)
                    {
                        
                        if(item.ClassId == itemChecked.ClassId && item.MaxCheckin > itemChecked.CountUsed)
                        {
                            modelRes.customerModel = item;
                            isMaxCheckin = false;
                            break;
                        }
                        if (item.ClassId == itemChecked.ClassId)
                        {
                            isBooking = true;
                        }
                    }
                    if (!isBooking)
                    {
                        modelRes.customerModel = item;
                        isMaxCheckin = false;
                        break;
                    }
                }

                if (lstCheckedInOrCheckedOut != null && lstCheckedInOrCheckedOut.Count >= 0)
                {
                    //lstCheckedInOrCheckedOut.Count >= MaxCheckIn
                    if (isMaxCheckin)
                    {
                        // reach MaxCheckin
                        _logger.LogInformation("ElgBookings - CheckBookingAsync - Check BookingKYC: Reach Max CheckIn");
                        return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING, StatusCode.Success);

                    }
                    else
                    {
                        // Not reach MaxCheckIn -> Customer can use elounge
                        // Step2: Check Voucher is exist or not
                        if (modelRes.customerModel.MapVoucherId != null && modelRes.customerModel.MapVoucherId.Count() > 0)
                        {
                            // Voucher has enough quantity
                            responseBookingKYC.Data = responseBookingKYC.Data.Where(x => x.Status == STATUS_BOOKING).ToList();
                        }
                        else
                        {
                            // User does not have any voucher
                            responseBookingKYC.Data = responseBookingKYC.Data.Where(x => x.Status == STATUS_BOOKING).ToList();
                        }

                    }
                }
                else
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check lstCheckedInOrCheckedOut: FAIL");
                    return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING_KYC_CUSTOMER, StatusCode.Success);
                }

                modelRes.listBookingModel = responseBookingKYC.Data;

                return new ResponseObject<ElgCheckBookingModel>(modelRes, "Thành công", StatusCode.Success);
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

        public async Task<Response> CheckBookingV2Async(string query)
        {
            try
            {
                // Kiểm tra nếu tìm kiếm theo mã đặt chỗ thì mã này đã checkin hoặc checkout chưa? có đúng ngày hay không?
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.GET_BOOKING_INFO_BY_CODE");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pReservationcode", query, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgBookingsHandler.ExecuteProcOracleReturnRow(procName, dyParam, false) as ResponseObject<ElgBookingsBaseModel>;
                decimal elgCustId = 0;
                var custId = "";
                var reservationcode = "";
                var cifNum = "";
                decimal bookingId = 0;
                if (result != null && result.StatusCode == StatusCode.Success && result.Data != null)
                {
                    reservationcode = query;
                    query = string.IsNullOrEmpty(result?.Data?.CustId) ? result.Data.CifNum : result?.Data?.CustId;
                    cifNum = result.Data.CifNum;
                    elgCustId = result.Data.Elg_Cust_Id;
                    bookingId = result.Data.Id.HasValue ? result.Data.Id.Value : 0;
                    custId = result.Data.CustId;
                    var notValidStatus = new List<string>() { "CHECKIN", "CHECKOUT", "CANCEL" };
                    if (notValidStatus.Contains(result.Data.Status)) return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING_KYC_CUSTOMER, StatusCode.Success);
                    // Kiểm tra ngày đăng ký
                    var startToDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 1);
                    var endToDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
                    var isInTime = (DateTime.Compare(result.Data.BookingTime, startToDay) >= 0 && DateTime.Compare(result.Data.BookingTime, endToDay) <= 0);
                    if (!isInTime) return new ResponseObject<ElgCheckBookingModel>(null, "Mã đặt chỗ đã quá ngày hoặc chưa đến ngày đăng ký", StatusCode.Success);
                }

                _elgCustomerHandler = new ElgCustomerHandler();
                ElgCheckBookingModel modelRes = new ElgCheckBookingModel();
                // KYC Customer first by query
                // lay danh sach quyen loi khach hang
                var respCustomerKYC = await _elgCustomerHandler.KYCCustomerV2Async(query, elgCustId) as ResponseObject<List<ElgCustomerBaseModel>>;

                if (respCustomerKYC?.Data == null)
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check CustomerKYC: FAIL");

                    return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING_KYC_CUSTOMER, StatusCode.Success);
                }
                
                modelRes.customerModel = respCustomerKYC.Data.FirstOrDefault();
                modelRes.listCustomerModel = respCustomerKYC.Data;
                // Get cust info
                ElgCustomerInfoHandler _elgCustomerInfoHandler = new ElgCustomerInfoHandler();
                //var getInfoById = await _elgCustomerInfoHandler.GetByCustIdAsync(modelRes.customerModel.CustId) as ResponseObject<ElgCustomerInfoModel>;
                //if (getInfoById != null) modelRes.customerModel.AllowPrivateRoom = getInfoById.Data.AllowPrivateRoom;
                // Get MaxCheckIn
                //decimal MaxCheckIn = modelRes.customerModel.MaxCheckin;
                // KYC Booking by custId
                var listCustId = modelRes.listCustomerModel.Select(x => x.CustId).Distinct();
                string stringListCustId = string.Join(",", listCustId);
                var responseBookingKYC = await KYCBookingV2Async(stringListCustId) as ResponseObject<List<ElgBookingsBaseModel>>;

                // Lấy danh sách hạng khách hàng theo đối tác Bông Sen Vàng
                var procName1 = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKIN.PRC_GET_LIST_CLASS_BY_PARTNER");
                var dyParam1 = new OracleDynamicParameters();
                dyParam1.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam1.Add("p_Partner_Code", PARTNER_CODE, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result1 = await _elgPartnerClassHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<ElgPartnerClass>>;
                if (result1 != null && result1.StatusCode == StatusCode.Success) modelRes.LstElgPartnerClass = result1.Data;

                if (responseBookingKYC?.Data == null)
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check BookingKYC: FAIL");
                    return new ResponseObject<ElgCheckBookingModel>(modelRes, "Thành công", StatusCode.Success);
                }
                _logger.LogInformation("ElgBookings - CheckBookingAsync - RES:" + JsonConvert.SerializeObject(responseBookingKYC));
                // Step1: Check list booking over maxcheckin or not
                List<ElgBookingsBaseModel> lstCheckedInOrCheckedOut = responseBookingKYC.Data.Where(x => STATUS_CHECKIN.Equals(x.Status) || STATUS_CHECKOUT.Equals(x.Status)).ToList();

                var checkedInByClassId = lstCheckedInOrCheckedOut.GroupBy(x => new { x.ClassId, x.Elg_Cust_Id }).Select(g => new { ClassId = g.Key.ClassId, ElgCustId = g.Key.Elg_Cust_Id, CountUsed = g.Count() }).ToList();
                var isMaxCheckin = true;
                var listCustomerModel = new List<ElgCustomerBaseModel>();
                foreach (var item in respCustomerKYC.Data)
                {
                    var isBooking = false;
                    foreach (var itemChecked in checkedInByClassId)
                    {
                        if (item.ClassId == itemChecked.ClassId && item.MaxCheckin > itemChecked.CountUsed)
                        {
                            //listCustomerModel.Add(item);
                            isMaxCheckin = false;
                            break;
                        }
                        if (item.ClassId == itemChecked.ClassId)
                        {
                            isBooking = true;
                        }
                    }
                    if (!isBooking)
                    {
                        listCustomerModel.Add(item);
                        isMaxCheckin = false;
                        //break;
                    }
                }

                modelRes.listCustomerModel = listCustomerModel;

                if (lstCheckedInOrCheckedOut != null && lstCheckedInOrCheckedOut.Count >= 0)
                {
                    //lstCheckedInOrCheckedOut.Count >= MaxCheckIn
                    if (isMaxCheckin)
                    {
                        // reach MaxCheckin
                        _logger.LogInformation("ElgBookings - CheckBookingAsync - Check BookingKYC: Reach Max CheckIn");
                        return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING, StatusCode.Success);

                    }
                    else
                    {
                        // Not reach MaxCheckIn -> Customer can use elounge
                        // Step2: Check Voucher is exist or not
                        var listBooking = responseBookingKYC.Data.Where(s => s.Status == STATUS_CHECKIN).ToList();
                        if (modelRes.customerModel.MapVoucherId != null && modelRes.customerModel.MapVoucherId.Count() > 0)
                        {
                            // Voucher has enough quantity
                            responseBookingKYC.Data = responseBookingKYC.Data.Where(x => x.Status == STATUS_BOOKING).ToList();
                        }
                        else
                        {
                            // User does not have any voucher
                            responseBookingKYC.Data = responseBookingKYC.Data.Where(x => x.Status == STATUS_BOOKING).ToList();
                        }

                        if (responseBookingKYC.Data.Count==0 && listBooking.Count>0 && !string.IsNullOrEmpty(modelRes.customerModel.CifNum))
                        {
                            responseBookingKYC.Data = listBooking;
                        }

                    }
                }
                else
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check lstCheckedInOrCheckedOut: FAIL");
                    return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING_KYC_CUSTOMER, StatusCode.Success);
                }

                modelRes.listBookingModel = responseBookingKYC.Data;

                return new ResponseObject<ElgCheckBookingModel>(modelRes, "Thành công", StatusCode.Success);
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

        //vinhtq:26/05/2021
        public async Task<Response> GetBookingsCheckinFilterAsync(ElgCheckInQueryModel model)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_FILTER_BOOKING_CHECKIN");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_cusname", model.CusName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_cus_id", model.CusId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_flight_tike_num", model.FlightTikeNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                var result = await _elgBookingsCheckinHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);
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

        //vinhtq::28/05/2021
        public async Task<Response> UpdateBookingCustomerBehavior(decimal bookingId, decimal isAddBehavior,string updateBy)
        {
            try
            {
                _logger.LogError("UpdateBookingCustomerBehavior", "Exception Error: " + bookingId + ";" + "isAddBehavior" + ";updateBy") ;
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_BOOKING.PRC_UPDATE_CUSTOMER_BEHAVIOR");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("i_booking_id", bookingId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_add_behavior", isAddBehavior, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_updateBy", isAddBehavior, OracleMappingType.NVarchar2, ParameterDirection.Input);
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                var result = await _elgBookingsHandler.ExecuteProcOracle(procName, dyParam);
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


        //vinhtq::08/12/2021
        //Hàm search khách hàng clone theo hàm search v2 của anh tungck
        public async Task<Response> CheckBookingV3Async(decimal elgCustId)
        {
            try
            {

                _elgCustomerHandler = new ElgCustomerHandler();
                ElgCheckBookingModel modelRes = new ElgCheckBookingModel();
                // KYC Customer first by query
                // lay danh sach quyen loi khach hang
                var respCustomerKYC = await _elgCustomerHandler.KYCCustomerV2Async(string.Empty, elgCustId) as ResponseObject<List<ElgCustomerBaseModel>>;

                if (respCustomerKYC?.Data == null)
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check CustomerKYC: FAIL");

                    return new ResponseObject<ElgCheckBookingModel>(null, "Không tìm thấy thông tin khách hàng hoặc khách hàng đã hết hạn sử dụng.Vui lòng nhờ IT hỗ trợ", StatusCode.Fail);
                }

                modelRes.customerModel = respCustomerKYC.Data.FirstOrDefault();
                modelRes.listCustomerModel = respCustomerKYC.Data;

                //Lấy thông tin face customer
                var  _elgFaceCustomerHandler = new ElgFaceCustomerHandler();
                var responseFaceCustomer = await _elgFaceCustomerHandler.GetByCustIdAsync(modelRes.customerModel.CustId) as ResponseObject<ElgFaceCustomerViewModel>;
                if (responseFaceCustomer!=null && responseFaceCustomer.Data!=null)
                {
                    modelRes.FaceCustomer = responseFaceCustomer.Data;
                }

                // Get cust info
                ElgCustomerInfoHandler _elgCustomerInfoHandler = new ElgCustomerInfoHandler();
                //var getInfoById = await _elgCustomerInfoHandler.GetByCustIdAsync(modelRes.customerModel.CustId) as ResponseObject<ElgCustomerInfoModel>;
                //if (getInfoById != null) modelRes.customerModel.AllowPrivateRoom = getInfoById.Data.AllowPrivateRoom;
                // Get MaxCheckIn
                //decimal MaxCheckIn = modelRes.customerModel.MaxCheckin;
                // KYC Booking by custId
                var listCustId = modelRes.listCustomerModel.Select(x => x.CustId).Distinct();
                string stringListCustId = string.Join(",", listCustId);
                var responseBookingKYC = await KYCBookingV2Async(stringListCustId) as ResponseObject<List<ElgBookingsBaseModel>>;

                // Lấy danh sách hạng khách hàng theo đối tác Bông Sen Vàng
                var procName1 = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_CHECKIN.PRC_GET_LIST_CLASS_BY_PARTNER");
                var dyParam1 = new OracleDynamicParameters();
                dyParam1.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam1.Add("p_Partner_Code", PARTNER_CODE, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result1 = await _elgPartnerClassHandler.ExecuteProcOracleReturnRow(procName1, dyParam1, true) as ResponseObject<List<ElgPartnerClass>>;
                if (result1 != null && result1.StatusCode == StatusCode.Success) modelRes.LstElgPartnerClass = result1.Data;

                if (responseBookingKYC?.Data == null)
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check BookingKYC: FAIL");
                    return new ResponseObject<ElgCheckBookingModel>(modelRes, "Thành công", StatusCode.Success);
                }
                _logger.LogInformation("ElgBookings - CheckBookingAsync - RES:" + JsonConvert.SerializeObject(responseBookingKYC));
                // Step1: Check list booking over maxcheckin or not
                List<ElgBookingsBaseModel> lstCheckedInOrCheckedOut = responseBookingKYC.Data.Where(x => STATUS_CHECKIN.Equals(x.Status) || STATUS_CHECKOUT.Equals(x.Status)).ToList();

                var checkedInByClassId = lstCheckedInOrCheckedOut.GroupBy(x => new { x.ClassId, x.Elg_Cust_Id }).Select(g => new { ClassId = g.Key.ClassId, ElgCustId = g.Key.Elg_Cust_Id, CountUsed = g.Count() }).ToList();
                var isMaxCheckin = true;
                var listCustomerModel = new List<ElgCustomerBaseModel>();
                foreach (var item in respCustomerKYC.Data)
                {
                    var isBooking = false;
                    foreach (var itemChecked in checkedInByClassId)
                    {
                        if (item.ClassId == itemChecked.ClassId && item.MaxCheckin > itemChecked.CountUsed)
                        {
                            //listCustomerModel.Add(item);
                            isMaxCheckin = false;
                            break;
                        }
                        if (item.ClassId == itemChecked.ClassId)
                        {
                            isBooking = true;
                        }
                    }
                    if (!isBooking)
                    {
                        listCustomerModel.Add(item);
                        isMaxCheckin = false;
                        //break;
                    }
                }

                modelRes.listCustomerModel = listCustomerModel;

                if (lstCheckedInOrCheckedOut != null && lstCheckedInOrCheckedOut.Count >= 0)
                {
                    //lstCheckedInOrCheckedOut.Count >= MaxCheckIn
                    if (isMaxCheckin)
                    {
                        // reach MaxCheckin
                        _logger.LogInformation("ElgBookings - CheckBookingAsync - Check BookingKYC: Reach Max CheckIn: Hạng khách hàng đã max lượt checkin");
                        return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING, StatusCode.Fail);

                    }
                    else
                    {
                        // Not reach MaxCheckIn -> Customer can use elounge
                        // Step2: Check Voucher is exist or not
                        var listBooking = responseBookingKYC.Data.Where(s => s.Status == STATUS_CHECKIN).ToList();
                        if (modelRes.customerModel.MapVoucherId != null && modelRes.customerModel.MapVoucherId.Count() > 0)
                        {
                            // Voucher has enough quantity
                            responseBookingKYC.Data = responseBookingKYC.Data.Where(x => x.Status == STATUS_BOOKING).ToList();
                        }
                        else
                        {
                            // User does not have any voucher
                            responseBookingKYC.Data = responseBookingKYC.Data.Where(x => x.Status == STATUS_BOOKING).ToList();
                        }

                        if (responseBookingKYC.Data.Count == 0 && listBooking.Count > 0 && !string.IsNullOrEmpty(modelRes.customerModel.CifNum))
                        {
                            responseBookingKYC.Data = listBooking;
                        }

                    }
                }
                else
                {
                    _logger.LogInformation("ElgBookings - CheckBookingAsync - Check lstCheckedInOrCheckedOut: FAIL");
                    return new ResponseObject<ElgCheckBookingModel>(null, ERROR_BOOKING_KYC_CUSTOMER, StatusCode.Fail);
                }

                modelRes.listBookingModel = responseBookingKYC.Data;

                return new ResponseObject<ElgCheckBookingModel>(modelRes, "Thành công", StatusCode.Success);
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
