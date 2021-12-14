using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                context.HttpContext.Request.Headers.TryGetValue("X-PermissionToken", out StringValues permissionTokenEncrypt);
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(permissionTokenEncrypt))
                {
                    // Giai ma token theo khoa 
                    var permissionTokenDecrypt = EncryptedString.DecryptString(permissionTokenEncrypt, Helpers.GetConfig("Encrypt:Key"));
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
        CheckPermission
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
