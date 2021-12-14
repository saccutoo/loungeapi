using API.Models;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Utils;

namespace API.BussinessLogic
{
    public class EmailHandler
    {
        private readonly string _urlCCommsEndpoint;
        //private readonly string _webUrl;
        private readonly bool _isUseOTPSms;
        private readonly string _smsOTPContentVI;
        private readonly string _smsOTPContentEN;
        private readonly string _accessToken;
        private readonly string _appSource;

        private readonly ILogger<EmailHandler> _logger;

        public EmailHandler(ILogger<EmailHandler> logger = null)
        {
            _logger = logger;
            _urlCCommsEndpoint = Helpers.GetConfig("CCommSConfig:UrlEndpoint");
            _appSource = Helpers.GetConfig("CCommSConfig:APP_SRC");
            _accessToken = Helpers.GetConfig("CCommSConfig:AccessToken");
            //_webUrl = Helpers.GetConfig("CCommSConfig:WebUrl");
            _isUseOTPSms = bool.Parse(Helpers.GetConfig("CCommSConfig:IsUseOTPSms"));
            _smsOTPContentVI = Helpers.GetConfig("CCommSConfig:SmsOTPContentVI");
            _smsOTPContentEN = Helpers.GetConfig("CCommSConfig:SmsOTPContentEN");
        }

        public async Task<Response> SendMailSuccessfullyBooking(string email, string userFullName, string reservationCode, string lang = "vi")
        {
            try
            {
                string baseTemplate = Directory.GetCurrentDirectory() + "/Attachs/EmailTemplate/EloungeBooking/";
                string templateFile = "";
                string title = "";
                if (lang == "en")
                {
                    title = "Confirmation from \"Air Lounge SHB\" system";
                    templateFile = baseTemplate + "templateEmailSuccessfullyBooking-EN.html";
                }

                if (lang == "vi")
                {
                    title = "Xác nhận từ hệ thống \"Phòng Chờ SHB tại Sân Bay Nôi Bài\"";
                    templateFile = baseTemplate + "templateEmailSuccessfullyBooking-VN.html";
                }
                var contentMail = await File.ReadAllTextAsync(templateFile);
                contentMail = contentMail.Replace("HO_VA_TEN", userFullName);
                contentMail = contentMail.Replace("MA_DAT_CHO", reservationCode);
                //contentMail = contentMail.Replace("@headerMailImage", _headerMailImage);
                //contentMail = contentMail.Replace("@footerMailImage", _footerMailImage);

                _ = SendEmail(email, title, contentMail,"SUCCESS_BOOKING");
                return new Response(StatusCode.Success, "Gửi email thành công");
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
        public async Task<Response> SendMailSuccessfullyBooking(ElgEmailBookingModel emailModel, string lang = "vi")
        {
            try
            {
                string baseTemplate = Directory.GetCurrentDirectory() + "/Attachs/EmailTemplate/EloungeBooking/";
                string templateFile = "";
                string title = "";
                if (lang == "en")
                {
                    title = "Confirmation from \"Air Lounge SHB\" system";
                    templateFile = baseTemplate + "templateEmailSuccessfullyBooking-EN.html";
                }

                if (lang == "vi")
                {
                    title = "Xác nhận từ hệ thống \"Phòng Chờ SHB tại Sân Bay Nội Bài\"";
                    templateFile = baseTemplate + "templateEmailSuccessfullyBooking-VN.html";
                }
                var contentMail = await File.ReadAllTextAsync(templateFile);
                contentMail = contentMail.Replace("HO_VA_TEN", emailModel.UserFullName);
                contentMail = contentMail.Replace("MA_DAT_CHO", emailModel.ReservationCode);
                contentMail = contentMail.Replace("SO_DIEN_THOAI", emailModel.PhoneNumber);
                contentMail = contentMail.Replace("SO_NGUOI_DI_KEM", emailModel.PersonGoWith);
                contentMail = contentMail.Replace("THOI_GIAN_DAT_CHO", emailModel.BookingTime);

                _ = SendEmail(emailModel.Email, title, contentMail,"SUCCESS_BOOKING");
                return new Response(StatusCode.Success, "Gửi email thành công");
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
        public async Task<Response> SendSMSOTPCancelBooking(string email, string userFullName, string phone, string reservationCode, string lang = "vi")
        {
            try
            {
                // Send SMS
                string smsOTP = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                string smsContent = "";
                if (lang == "vi") smsContent = _smsOTPContentVI.Replace("@OTP", smsOTP);
                else if (lang == "en") smsContent = _smsOTPContentEN.Replace("@OTP", smsOTP);
                if (_isUseOTPSms) _ = await SendSMSOTP(phone, smsContent);
                else _ = await SendMailOTP(email, userFullName, smsOTP, reservationCode, lang);

                return new Response(StatusCode.Success, "Gửi OTP thành công");
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
        public async Task<Response> SendMailCancelBooking(string email, string userFullName, string reservationCode, string lang = "vi")
        {
            try
            {
                string baseTemplate = Directory.GetCurrentDirectory() + "/Attachs/EmailTemplate/EloungeBooking/";
                string templateFile = "";
                string title = "";
                if (lang == "en")
                {
                    title = "Confirmation from system \"Air Lounge SHB\"";
                    templateFile = baseTemplate + "templateEmailCancelBooking-EN.html";
                }

                if (lang == "vi")
                {
                    title = "Xác nhận từ hệ thống \"Phòng chờ sân bay SHB\"";
                    templateFile = baseTemplate + "templateEmailCancelBooking-VN.html";
                }
                var contentMail = await File.ReadAllTextAsync(templateFile);
                contentMail = contentMail.Replace("@fullName", userFullName);
                contentMail = contentMail.Replace("@reservationCode", reservationCode);
                //contentMail = contentMail.Replace("@headerMailImage", _headerMailImage);
                //contentMail = contentMail.Replace("@footerMailImage", _footerMailImage);

                _ = SendEmail(email, title, contentMail,"CANCEL_BOOKING");
                return new Response(StatusCode.Success, "Gửi email thành công");
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
        public async Task<Response> SendMailCancelBooking(ElgEmailBookingModel emailModel, string lang = "vi")
        {
            try
            {
                string baseTemplate = Directory.GetCurrentDirectory() + "/Attachs/EmailTemplate/EloungeBooking/";
                string templateFile = "";
                string title = "";
                if (lang == "en")
                {
                    title = "Confirmation from system \"Air Lounge SHB\"";
                    templateFile = baseTemplate + "templateEmailCancelBooking-EN.html";
                }

                if (lang == "vi")
                {
                    title = "Xác nhận từ hệ thống \"Phòng Chờ SHB tại Sân Bay Nôi Bài\"";
                    templateFile = baseTemplate + "templateEmailCancelBooking-VN.html";
                }
                var contentMail = await File.ReadAllTextAsync(templateFile);
                contentMail = contentMail.Replace("HO_VA_TEN", emailModel.UserFullName);
                contentMail = contentMail.Replace("MA_DAT_CHO", emailModel.ReservationCode);

                _ = SendEmail(emailModel.Email, title, contentMail,"CANCEL_BOOKING");
                return new Response(StatusCode.Success, "Gửi email thành công");
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
        private async Task<Response> SendEmail(string email, string title, string content,string function)
        {
            try
            {

                PushNotificationModel emailMessage = new PushNotificationModel
                {
                    ID = 10000,
                    NEWS_CONTENT = content,
                    PAYLOAD_DATA = "{}",
                    NEWS_TITLE = title,
                    NEWS_TO = email,
                    NEWS_COMM_TYPE = "EMAIL",
                    STATUS = 0,
                    PRIORITY = 0,
                    IS_VIEW_DETAIL = 0,
                    CREATE_DATE = DateTime.Now,
                    MODIFIED_DATE = DateTime.Now,
                    PARENT_ID = "",
                    APP_SRC = _appSource,
                    FUNC_SRC = function,
                    REF_ID = ""
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_urlCCommsEndpoint + "api/ccomms/immediate");

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                    var response = await client.PostAsJsonAsync(_urlCCommsEndpoint + "api/ccomms/immediate", emailMessage);
                    var stringResponse = response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return new Response(StatusCode.Success, "Success");
                    }
                    else return new Response(StatusCode.Fail, "Fail");
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
        private async Task<Response> SendSMSOTP(string phone, string content)
        {
            try
            {
                PushNotificationModel smsMessage = new PushNotificationModel
                {
                    ID = 0,
                    NEWS_CONTENT = content,
                    PAYLOAD_DATA = "",
                    NEWS_TITLE = "",
                    NEWS_TO = phone,
                    NEWS_COMM_TYPE = "SMS",
                    STATUS = 0,
                    PRIORITY = 0,
                    IS_VIEW_DETAIL = 0,
                    CREATE_DATE = DateTime.Now,
                    MODIFIED_DATE = DateTime.Now,
                    PARENT_ID = "",
                    APP_SRC = "SHB_MOBILE",
                    FUNC_SRC = "AIR_LOUNGE",
                    REF_ID = ""
                };

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_urlCCommsEndpoint + "api/ccomms/immediate");

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                    var response = await client.PostAsJsonAsync(_urlCCommsEndpoint + "api/ccomms/immediate", smsMessage);
                    var stringResponse = response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return new Response(StatusCode.Success, "Success");
                    }
                    else return new Response(StatusCode.Fail, "Fail");
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
        private async Task<Response> SendMailOTP(string email, string userFullName, string OTP, string reservationCode, string lang = "vi")
        {
            try
            {
                string baseTemplate = Directory.GetCurrentDirectory() + "/Attachs/EmailTemplate/EloungeBooking/";
                string templateFile = "";
                string title = "";
                if (lang == "en")
                {
                    title = "OTP from \"Air Lounge SHB\" system";
                    templateFile = baseTemplate + "templateEmailSMS-EN.html";
                }

                if (lang == "vi")
                {
                    title = "Mã xác thực OTP từ hệ thống \"Phòng chờ sân bay SHB\"";
                    templateFile = baseTemplate + "templateEmailSMS-VN.html";

                }
                var contentMail = await File.ReadAllTextAsync(templateFile);
                contentMail = contentMail.Replace("@fullName", userFullName);
                contentMail = contentMail.Replace("@reservationCode", reservationCode);
                contentMail = contentMail.Replace("@OTP", OTP);
                //contentMail = contentMail.Replace("@headerMailImage", _headerMailImage);
                //contentMail = contentMail.Replace("@footerMailImage", _footerMailImage);

                _ = SendEmail(email, title, contentMail,"MAIL_OTP");
                return new Response(StatusCode.Success, "Gửi email thành công");
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

        public async Task<Response> SendMailSuccessfullyVoucher(string email, string userFullName, string voucherSeria, string truocNgay, string lang = "vi")
        {
            try
            {
                string baseTemplate = Directory.GetCurrentDirectory() + "/Attachs/EmailTemplate/EloungeBooking/";
                string templateFile = "";
                string title = "";
                if (lang == "en")
                {
                    title = "Confirmation from \"Air Lounge SHB\" system";
                    templateFile = baseTemplate + "templateEmailSuccessfullyVoucher.html";
                }

                if (lang == "vi")
                {
                    title = "Xác nhận từ hệ thống \"Phòng Chờ SHB tại Sân Bay Nôi Bài\"";
                    templateFile = baseTemplate + "templateEmailSuccessfullyVoucher.html";
                }
                var contentMail = await File.ReadAllTextAsync(templateFile);
                contentMail = contentMail.Replace("HO_VA_TEN", userFullName);
                contentMail = contentMail.Replace("MA_VOUCHER", voucherSeria);
                contentMail = contentMail.Replace("TRUOC_NGAY", truocNgay);
                //contentMail = contentMail.Replace("@headerMailImage", _headerMailImage);
                //contentMail = contentMail.Replace("@footerMailImage", _footerMailImage);

                _ = SendEmail(email, title, contentMail,"MAP_VOUCHER");
                return new Response(StatusCode.Success, "Gửi email thành công");
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
