using API.BussinessLogic;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace API.Filters
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(TypeFilter typeFilter, string value) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { typeFilter, value };
        }
    }
    public class PermissionFilter : IAuthorizationFilter
    {
        private readonly TypeFilter _typeFilter;
        private readonly string _value;
        private readonly ILogger<PermissionFilter> _logger;
        public PermissionFilter(TypeFilter typeFilter, string value, ILogger<PermissionFilter> logger = null)
        {
            _typeFilter = typeFilter;
            _value = value;
            _logger = logger;
        }

        private string[] GetListPermissionFilter()
        {
            if (!string.IsNullOrEmpty(_value)) return _value.Split(",");
            return null;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                context.HttpContext.Request.Headers.TryGetValue("X-UserName", out StringValues userName);
                context.HttpContext.Request.Headers.TryGetValue("X-PermissionToken", out StringValues permissionToken);
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(permissionToken))
                {
                    #region Xac thuc qua JWT
                    if (_typeFilter == TypeFilter.Jwt)
                    {
                        // Lấy thông tin người dùng từ DB
                        ElgStarffHandler elgStarffHandler = new ElgStarffHandler();

                        var callStaffByUserName = Task.Run(() => elgStarffHandler.GetByUserNameAsync(userName));
                        callStaffByUserName.Wait();
                        var getStaffByUserNameRes = callStaffByUserName.Result as ResponseObject<ElgStarffBaseModel>;
                        if (getStaffByUserNameRes.StatusCode == StatusCode.Success)
                        {

                            // Kiểm tra người dùng đã đăng xuất chưa
                            if (!string.IsNullOrEmpty(getStaffByUserNameRes.Data.JWT))
                            {
                                var securityKey = Helpers.GetConfig("JWTAuthentication:SecurityKey");
                                TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateLifetime = true,
                                    ValidateIssuer = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidateAudience=true,
                                    ValidIssuer = Helpers.GetConfig("JWTAuthentication:Issuer"),
                                    ValidAudience = Helpers.GetConfig("JWTAuthentication:Issuer"),
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
                                };

                                IJwtHandler jwtHandler = new JwtHandler(securityKey);
                                var isValidToken = jwtHandler.IsTokenValid(permissionToken, tokenValidationParameters);
                                if (!isValidToken) context.Result = new BadRequestObjectResult("Permission Denied");

                                // Kiểm tra phiên đăng nhập hợp lệ với JWT Id
                                List<Claim> claims = jwtHandler.GetTokenClaims(permissionToken).ToList();
                                var jwtId = claims.FirstOrDefault(t => t.Type.Equals("jti")).Value;
                                if (!string.Equals(getStaffByUserNameRes.Data.JWT, jwtId)) context.Result = new BadRequestObjectResult("Fake Session");

                            }
                            else context.Result = new BadRequestObjectResult("Session Expired");
                        }
                        else context.Result = new BadRequestObjectResult("Invalid Header");
                    }
                    #endregion
                    else
                    {
                        // Giai ma token theo khoa 
                        var permissionTokenDecrypt = EncryptedString.DecryptString(permissionToken, Helpers.GetConfig("Encrypt:Key"));
                        // Chuyen doi thanh model
                        PermissionTokenModel permissionTokenModel = JsonConvert.DeserializeObject<PermissionTokenModel>(permissionTokenDecrypt);
                        if (permissionTokenModel != null)
                        {
                            // Kiem tra thong tin userName tren header va trong permission token
                            if (permissionTokenModel.UserName != userName) context.Result = new BadRequestObjectResult("Invalid Header");
                            // Kiem tra thoi gian het han cua token
                            // else if (DateTime.Compare(DateTime.Now, permissionTokenModel.ExpiredIn) == 1) context.Result = new BadRequestObjectResult("Token Expired");
                            else
                            {
                                if (permissionTokenModel.ListFuncPerms != null && permissionTokenModel.ListFuncPerms.Count > 0)
                                {
                                    // Lay tat ca quyen cua user
                                    var lstPermissionOfUser = permissionTokenModel.ListFuncPerms.Select(sp => sp.FuncPermCode).ToList();
                                    // Lay tat ca quyen duoc thuc thi controller
                                    var lstPermissionOfController = GetListPermissionFilter();

                                    var checkPer = lstPermissionOfUser.Exists(sp => lstPermissionOfController.Contains(sp));
                                    if (!checkPer) context.Result = new BadRequestObjectResult("Permission Denied");
                                }
                            }
                        }
                        else context.Result = new BadRequestObjectResult("Invalid Header");
                    }
                }
                else context.Result = new BadRequestObjectResult("Invalid Header");
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError(ex, "Exception Error");
                }
                context.Result = new BadRequestObjectResult("Invalid Header");
            }
        }
    }
    public enum TypeFilter
    {
        CheckPermission,
        Jwt
    }
    /// <summary>
    /// Model chứa thông tin quyền hạn truy cập của 
    /// người dùng, thông tin được mã hóa và truyền lên trên header của request
    /// </summary>
    public class PermissionTokenModel
    {
        public decimal ExpiredAfter { get; set; }
        public DateTime ExpiredIn { get; set; }
        public string UserName { get; set; }
        public List<FuncLoginModel> ListFuncPerms { get; set; }
    }
    public class FuncLoginModel
    {
        public decimal? FunctionId { get; set; }
        public string FuncPermCode { get; set; }
    }
}
