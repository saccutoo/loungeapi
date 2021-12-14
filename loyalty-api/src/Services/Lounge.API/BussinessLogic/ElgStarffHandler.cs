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

namespace API.BussinessLogic
{
    public class ElgStarffHandler : IElgStarffHandler
    {
        private readonly RepositoryHandler<ElgStaff, ElgStarffBaseModel, ElgStarffQueryModel> _elgStarffHandler
               = new RepositoryHandler<ElgStaff, ElgStarffBaseModel, ElgStarffQueryModel>();
        private readonly string _dBSchemaName;
        private readonly ILogger<ElgStarffHandler> _logger;
        private readonly decimal? _maxFailedCount;
        private readonly decimal? _lockTimeFailed;
        private readonly decimal? _visibleCaptchaCount;
        private readonly decimal? _sessionTime;
        private string _apiGatewayRootUrl;
        private string _clientId;
        private string _clientSecret;

        public ElgStarffHandler(ILogger<ElgStarffHandler> logger = null)
        {
            _logger = logger;
            _dBSchemaName = Helpers.GetConfig("DBSchemaName");
            _maxFailedCount = decimal.Parse(Helpers.GetConfig("LoginConfig:MaxFailedCount"));
            _lockTimeFailed = decimal.Parse(Helpers.GetConfig("LoginConfig:LockTimeFailed"));
            _visibleCaptchaCount = decimal.Parse(Helpers.GetConfig("LoginConfig:VisibleCaptchaCount"));
            _sessionTime = decimal.Parse(Helpers.GetConfig("LoginConfig:SessionTime"));
        }
        public async Task<Response> CreateAsync(ElgStarffCreateModel model, ELoungeBaseModel baseModel)
        {
            try
            {

                _logger.LogInformation("ElgStarff - CreateAsync  - REQ: model: " + JsonConvert.SerializeObject(model) + "|baseModel: " + JsonConvert.SerializeObject(baseModel));
                // Mã hóa mật khẩu
                var passwordDecrypt = EncryptedString.DecryptString(model.Password, Helpers.GetConfig("Encrypt:Key"));
                var salt = Helpers.PasswordCreateSalt512();
                var passEncrypt = Helpers.PasswordGenerateHmac(passwordDecrypt, salt);

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.PRC_CREATE_STAFF");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_username", model.UserName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_fullname", model.FullName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_personal_id", model.PersonalId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phone_num", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_email", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_gender", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_lounge_id", model.LoungeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.CreateBy, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_password", passEncrypt, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_password_salt", salt, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("ElgStarff - CreateAsync - RES:" + JsonConvert.SerializeObject(result));

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
        public async Task<Response> UpdateAsync(decimal id, ElgStarffUpdateModel model, ELoungeBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("ElgStarff - UpdateAsync - REQ: id: " + id + "|model: " + JsonConvert.SerializeObject(model) + "|baseModel: " + JsonConvert.SerializeObject(baseModel));

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.PRC_UPDATE_STAFF");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_staff_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_fullname", model.FullName, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_personal_id", model.PersonalId, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_phone_num", model.PhoneNum, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_email", model.Email, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_gender", model.Gender, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_lounge_id", model.LoungeId, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("ElgStarff - UpdateAsync - RES:" + JsonConvert.SerializeObject(result));

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
        public async Task<Response> UpdateLoginAsync(decimal id, ElgStarfLoginUpdateModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.PRC_UPDATE_LOGIN");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_staff_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_failed_pass_count", model.FailedPassCount, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_last_login_date", model.LastLoginDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_last_logout_date", model.LastLogoutDate, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_lock_time", model.LockTime, OracleMappingType.Date, ParameterDirection.Input);
                dyParam.Add("i_status", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_jwt", model.JWT, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_refresh_token", model.RefreshToken, OracleMappingType.Varchar2, ParameterDirection.Input);
                var result = await _elgStarffHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("ElgStarff - UpdateAsync - RES:" + JsonConvert.SerializeObject(result));

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
        public async Task<Response> UpdatePassAsync(decimal id, ElgStarfLoginUpdateModel model)
        {
            try
            {

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.PRC_UPDATE_PASS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_staff_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_password", model.Password, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_password_salt", model.PasswordSalt, OracleMappingType.Varchar2, ParameterDirection.Input);


                var result = await _elgStarffHandler.ExecuteProcOracle(procName, dyParam);

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
        public async Task<Response> GetByFilterAsync(ElgStarffQueryModel model)
        {
            try
            {
                _logger.LogInformation("ElgStarff - GetByFilterAsync - REQ: " + JsonConvert.SerializeObject(model));

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.PRC_FILTER_STAFF");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_configurate_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_page_size", model.PageSize, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_page_index", model.PageIndex, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_search_text", model.FullTextSearch, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_status_id", model.Status, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("ElgStarff - GetByFilterAsync - RES:" + JsonConvert.SerializeObject(result));

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
        public async Task<Response> GetByAllListAsync(string condition)
        {
            try
            {
                _logger.LogInformation("ElgStarff - GetByAllListAsync - REQ: " + condition);

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.PRC_GET_LIST_LOUNGES");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_data_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_text_search", condition, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracleReturnRow(procName, dyParam, true);

                _logger.LogInformation("ElgStarff - GetByAllListAsync - RES:" + JsonConvert.SerializeObject(result));

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
        public async Task<Response> GetByUserNameAsync(string userName)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.GET_BY_USER_NAME");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pUserName", userName, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);

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

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.GET_BY_ID");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pId", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracleReturnRow(procName, dyParam, false);

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
        public async Task<Response> LoginJWT(ElgStarfLoginModel loginModel)
        {
            try
            {
                ElgStarfLoginResponseModel userLoginResponse;
                string userName = loginModel != null ? loginModel.UserName : string.Empty;
                string passWord = loginModel != null ? loginModel.Password : string.Empty;
                if (!(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passWord)))
                {
                    var passwordPlaintext = EncryptedString.DecryptString(passWord, Helpers.GetConfig("Encrypt:Key"));
                    // Kiểm tra thông tin tên đăng nhập
                    var existUser = await GetByUserNameAsync(userName) as ResponseObject<ElgStarffBaseModel>;
                    if (existUser.StatusCode == StatusCode.Success && existUser.Data != null)
                    {
                        // Kiêm tra LoungeId của Staff
                        //if (!loginModel.LoungeId.HasValue || (loginModel.LoungeId.HasValue && loginModel.LoungeId.Value != existUser.Data.LoungeId))
                        //    return new ResponseObject<ElgStarfLoginResponseModel>(null, "Nhân viên không thuộc phòng chờ đang chọn", StatusCode.Fail);
                        var isValid = false;
                        // Kiểm tra mật khẩu
                        var userSalt = existUser.Data.PasswordSalt;
                        var userPass = existUser.Data.Password;
                        string inputHmac = string.Empty;
                        if (!string.IsNullOrEmpty(userSalt)) inputHmac = Helpers.PasswordGenerateHmac(passwordPlaintext, userSalt);

                        decimal? coutLoginFall = existUser.Data.FailedPassCount + 1;

                        decimal totalminutes = existUser.Data.LockTime.HasValue ? (decimal)(DateTime.Now - existUser.Data.LockTime.Value).TotalMinutes : _lockTimeFailed.Value + 1;

                        if (existUser.Data.Status.Equals("LOCKED") && totalminutes <= _lockTimeFailed)
                        {
                            return new ResponseObject<ElgStarfLoginResponseModel>(null, "Tài khoản hiện đang bị khóa", StatusCode.Fail);
                        }

                        // Kiểm tra mk trong DB
                        isValid = string.Equals(userPass, inputHmac);

                        if (isValid)
                        {
                            userLoginResponse = new ElgStarfLoginResponseModel();

                            var data = existUser?.Data;
                            var getAccessTokenResult = GetAccessToken().Result;
                            if (getAccessTokenResult != null) userLoginResponse.AccessToken = getAccessTokenResult.access_token;

                            userLoginResponse.Id = data.Id;
                            userLoginResponse.IsVisibleCaptcha = false;
                            userLoginResponse.SessionTime = _sessionTime ?? 20;

                            #region Tạo jwt token trả lại cho client
                            var jwtId = Guid.NewGuid().ToString();
                            var refreshToken = Guid.NewGuid().ToString();
                            IJwtContainerModel jwtModel = new JwtContainerModel
                            {
                                Claims = new Claim[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier,data.UserName),
                                    new Claim(ClaimTypes.Email,data.Email),
                                    new Claim("jti",jwtId)
                                }
                            };
                            IJwtHandler jwtHandler = new JwtHandler(jwtModel.SecretKey);
                            userLoginResponse.PermissionToken = jwtHandler.GenerateToken(jwtModel);
                            userLoginResponse.RefreshToken = refreshToken;
                            #endregion

                            // Cập nhật lại thông tin khi đăng nhập
                            var updateModel = new ElgStarfLoginUpdateModel
                            {
                                FailedPassCount = 0,
                                LastLoginDate = DateTime.Now,
                                LastLogoutDate = data.LastLogoutDate,
                                LockTime = null,
                                Status = data.Status == "LOCKED" ? "APPROVED" : data.Status,
                                JWT = jwtId,
                                RefreshToken = refreshToken
                            };

                            var updateUserSuccess = await UpdateLoginAsync(data.Id, updateModel);
                            return new ResponseObject<ElgStarfLoginResponseModel>(userLoginResponse, "Đăng nhập thành công");
                        }

                        else
                        {
                            string mess = "Tên đăng nhập hoặc mật khẩu không đúng";
                            userLoginResponse = new ElgStarfLoginResponseModel();
                            var updateLoginModel = new ElgStarfLoginUpdateModel
                            {
                                LastLoginDate = existUser.Data.LastLoginDate,
                                LastLogoutDate = existUser.Data.LastLogoutDate,
                                FailedPassCount = coutLoginFall,
                                Status = "APPROVED"
                            };

                            if (coutLoginFall >= _visibleCaptchaCount) userLoginResponse.IsVisibleCaptcha = true;
                            else userLoginResponse.IsVisibleCaptcha = false;

                            if (coutLoginFall >= _maxFailedCount)
                            {
                                updateLoginModel.LockTime = DateTime.Now;
                                updateLoginModel.Status = "LOCKED";
                                updateLoginModel.FailedPassCount = 0;
                                userLoginResponse.IsVisibleCaptcha = false;
                                mess = "Tài khoản hiện đang bị khóa";
                            }
                            else
                            {
                                updateLoginModel.LockTime = existUser.Data.LockTime;
                            }

                            var updateUserFail = await UpdateLoginAsync(existUser.Data.Id, updateLoginModel);

                            return new ResponseObject<ElgStarfLoginResponseModel>(null, mess, StatusCode.Fail);
                        }
                    }
                    else return new ResponseObject<ElgStarfLoginResponseModel>(null, "Tên đăng nhập hoặc mật khẩu không đúng", StatusCode.Fail);
                }
                else return new ResponseObject<ElgStarfLoginResponseModel>(null, "Tên đăng nhập hoặc mật khẩu không đúng", StatusCode.Fail);
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
        public async Task<Response> Logout(decimal id)
        {
            try
            {
                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.LOG_OUT");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("OUT_CUR", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("pId", id, OracleMappingType.Decimal, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracle(procName, dyParam);

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
        public async Task<Response> RefreshToken(string token, string refreshToken)
        {
            try
            {
                if(string.IsNullOrEmpty(token)) return new ResponseObject<ElgStarfLoginResponseModel>(null, "Token không hợp lệ", StatusCode.Fail);
                // Lấy thông tin claims từ token đúng chữ ký xác thực
                var securityKey = Helpers.GetConfig("JWTAuthentication:SecurityKey");
                TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidIssuer = Helpers.GetConfig("JWTAuthentication:Issuer"),
                    ValidAudience = Helpers.GetConfig("JWTAuthentication:Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
                };

                IJwtHandler jwtHandler = new JwtHandler(securityKey);
                var isValidToken = jwtHandler.IsTokenValid(token, tokenValidationParameters);
                if (!isValidToken) return new ResponseObject<ElgStarfLoginResponseModel>(null, "Token không hợp lệ", StatusCode.Fail);

                // Lấy thông tin claims từ token hợp lệ
                List<Claim> claims = jwtHandler.GetTokenClaims(token).ToList();
                var userName = claims.FirstOrDefault(t => t.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var existUser = await GetByUserNameAsync(userName) as ResponseObject<ElgStarffBaseModel>;
                if (existUser.StatusCode == StatusCode.Success)
                {
                    var userLoginResponse = new ElgStarfLoginResponseModel
                    {
                        Id = existUser.Data.Id
                    };
                    if (!string.Equals(refreshToken, existUser.Data.RefreshToken))
                        return new ResponseObject<ElgStarfLoginResponseModel>(null, "Refresh Token không hợp lệ", StatusCode.Fail);

                    // Khởi tạo lại token
                    var jwtId = Guid.NewGuid().ToString();
                    var newRefreshToken = Guid.NewGuid().ToString();
                    IJwtContainerModel jwtModel = new JwtContainerModel
                    {
                        Claims = new Claim[]
                        {
                                    new Claim(ClaimTypes.NameIdentifier,existUser.Data.UserName),
                                    new Claim(ClaimTypes.Email,existUser.Data.Email),
                                    new Claim("jti",jwtId)
                        }
                    };
                    // Lấy lại access token
                    var getAccessTokenResult = GetAccessToken().Result;
                    if (getAccessTokenResult != null) userLoginResponse.AccessToken = getAccessTokenResult.access_token;

                    userLoginResponse.PermissionToken = jwtHandler.GenerateToken(jwtModel);
                    userLoginResponse.RefreshToken = newRefreshToken;

                    return new ResponseObject<ElgStarfLoginResponseModel>(userLoginResponse, string.Empty, StatusCode.Success);
                }
                else return new ResponseObject<ElgStarfLoginResponseModel>(null, "Thông tin người dùng không hợp lệ", StatusCode.Fail);
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
        public async Task<Response> ChangePassword(decimal id, string oldPass, string newPass, bool force = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(oldPass) && !string.IsNullOrEmpty(newPass))
                {
                    var existUser = await GetByIdAsync(id) as ResponseObject<ElgStarffBaseModel>;
                    var oldPasswordPlaintext = EncryptedString.DecryptString(oldPass, Helpers.GetConfig("Encrypt:Key"));
                    var newPasswordPlaintext = EncryptedString.DecryptString(newPass, Helpers.GetConfig("Encrypt:Key"));

                    if (existUser.StatusCode == StatusCode.Success && existUser.Data != null)
                    {
                        var userOldSalt = existUser.Data.PasswordSalt;
                        var userOldPass = existUser.Data.Password;

                        // Kiểm tra mk cũ
                        if (!force && !string.Equals(userOldPass, Helpers.PasswordGenerateHmac(oldPasswordPlaintext, userOldSalt)))
                            return new ResponseObject<ElgStarfLoginResponseModel>(null, "Mật khẩu cũ chưa đúng", StatusCode.Fail);

                        // Mã hóa mật khẩu mới
                        var salt = Helpers.PasswordCreateSalt512();
                        var newPassEncrypt = Helpers.PasswordGenerateHmac(newPasswordPlaintext, salt);

                        var updatePass = await UpdatePassAsync(id, new ElgStarfLoginUpdateModel { Password = newPassEncrypt, PasswordSalt = salt });
                        return new ResponseObject<ElgStarfLoginResponseModel>(null, "Thay đổi mật khẩu thành công", StatusCode.Success);
                    }
                    else return new ResponseObject<ElgStarfLoginResponseModel>(null, "Tên đăng nhập hoặc mật khẩu không đúng", StatusCode.Fail);
                }
                return new ResponseObject<ElgStarfLoginResponseModel>(null, "Chưa nhập đủ mật khẩu cũ và mới", StatusCode.Fail);
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
        public async Task<AuthModel> GetAccessToken()
        {
            try
            {

                _apiGatewayRootUrl = Helpers.GetConfig("Authenticate:APIGatewayRootUrl");
                _clientId = Helpers.GetConfig("Authenticate:ClientId");
                _clientSecret = Helpers.GetConfig("Authenticate:ClientSecret");

                Uri getAccessTokenUri = new Uri(_apiGatewayRootUrl + "/token")
                    .AddQuery("grant_type", "client_credentials");

                var getAccessTokenRequest = (HttpWebRequest)WebRequest.Create(getAccessTokenUri);
                getAccessTokenRequest.Method = "POST";
                string basicToken = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(_clientId + ":" + _clientSecret));
                getAccessTokenRequest.ContentType = "application/x-www-form-urlencoded";
                getAccessTokenRequest.Accept = "application/json, text/javascript, */*";
                getAccessTokenRequest.Headers.Add("Authorization", "Basic " + basicToken);

                using (var getAccessTokenResponse = (HttpWebResponse)getAccessTokenRequest.GetResponse())
                {
                    var streamResponse = getAccessTokenResponse.GetResponseStream();
                    MemoryStream ms = new MemoryStream();
                    streamResponse.CopyTo(ms);
                    ms.Position = 0;
                    HttpResponseMessage resultResponse = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(ms)
                    };
                    resultResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    string resultResponseStringData = await resultResponse.Content.ReadAsStringAsync();

                    var resultResponseJsonData = JsonConvert.DeserializeObject<AuthModel>(resultResponseStringData);
                    return resultResponseJsonData;
                }

            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                    return null;
                }
                else throw ex;
            }
        }
        public async Task<Response> UpdateStatusAsync(decimal id, string status, ELoungeBaseModel baseModel)
        {
            try
            {
                _logger.LogInformation("ElgStarff - UpdateStatusAsync - REQ: id: " + id + "|status: " + status);

                var procName = string.Join('.', _dBSchemaName, "PKG_ELG_CMS_STAFF.PRC_UPDATE_STAFF_STATUS");
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("o_resp_cursor", null, OracleMappingType.RefCursor, ParameterDirection.Output);
                dyParam.Add("i_staff_id", id, OracleMappingType.Decimal, ParameterDirection.Input);
                dyParam.Add("i_new_status", status, OracleMappingType.Varchar2, ParameterDirection.Input);
                dyParam.Add("i_create_by", baseModel.LastModifiedBy, OracleMappingType.Varchar2, ParameterDirection.Input);

                var result = await _elgStarffHandler.ExecuteProcOracle(procName, dyParam);

                _logger.LogInformation("ElgStarff - UpdateStatusAsync - RES:" + JsonConvert.SerializeObject(result));

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
