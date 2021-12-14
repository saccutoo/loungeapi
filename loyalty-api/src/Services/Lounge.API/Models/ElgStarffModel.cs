using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

namespace API.Models
{
    public class ElgStarffBaseModel : ELoungeBaseModel
    {
        public decimal Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public decimal LoungeId { get; set; }
        public string PersonalId { get; set; }
        public string PhoneNum { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public int? FailedPassCount { get; set; }
        public DateTime? LockTime { get; set; }
        public decimal OrderView { get; set; }
        public string Status { get; set; }
        public string JWT { get; set; }
        public string RefreshToken { get; set; }
        public string LoungeName { get; set; }
        public string LoungeAddress { get; set; }
    }
    public class ElgStarffCreateModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PersonalId { get; set; }
        public string PhoneNum { get; set; }
        public decimal LoungeId { get; set; }
    }
    public class ElgStarffUpdateModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string PersonalId { get; set; }
        public string PhoneNum { get; set; }
        public decimal LoungeId { get; set; }
        public string Status { get; set; }
    }

    public class ElgStarffQueryModel : PaginationRequest
    {
        public string Status { get; set; }
    }

    #region AUTH
    public class ElgStarfLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        //public decimal? LoungeId { get; set; }

    }
    public class ElgStarfChangePasswordModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class ElgStarfLoginResponseModel
    {
        public decimal? Id { get; set; }
        public decimal SessionTime { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string PermissionToken { get; set; }
        public bool IsVisibleCaptcha { get; set; }
    }
    public class ElgStarfLoginUpdateModel
    {
        public string Status { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public decimal? FailedPassCount { get; set; }
        public DateTime? LockTime { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string JWT { get; set; }
        public string RefreshToken { get; set; }
    }
    public class AuthModel
    {
        public string access_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public decimal expires_in { get; set; }
        public string refresh_token { get; set; }
    }
    public class JwtContainerModel : IJwtContainerModel
    {
        public int ExpireMinutes { get; set; } = int.Parse(Helpers.GetConfig("JWTAuthentication:ExpiryDuration"));
        public string SecretKey { get; set; } = Helpers.GetConfig("JWTAuthentication:SecurityKey");
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public Claim[] Claims { get; set; }
        public string Issuer { get; set; } = Helpers.GetConfig("JWTAuthentication:Issuer");
    }
    #endregion
}
