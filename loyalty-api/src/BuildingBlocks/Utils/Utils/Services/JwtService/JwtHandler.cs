using System;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using System.Text;

namespace Utils
{
    public class JwtHandler : IJwtHandler
    {
        #region Members
        /// <summary>
        /// The secret key we use to encrypt out token with.
        /// </summary>
        public string SecretKey { get; set; }
        #endregion

        #region Constructor
        public JwtHandler(string secretKey)
        {
            SecretKey = secretKey;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Validates whether a given token is valid or not, and returns true in case the token is valid otherwise it will return false;
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool IsTokenValid(string token, TokenValidationParameters tokenValidationParameters)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Generates token by given model.
        /// Validates whether the given model is valid, then gets the symmetric key.
        /// Encrypt the token and returns it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Generated token.</returns>
        public string GenerateToken(IJwtContainerModel model)
        {
            if (model == null || model.Claims == null || model.Claims.Length == 0)
                throw new ArgumentException("Arguments to create token are not valid.");

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(model.Claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(model.ExpireMinutes)),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), model.SecurityAlgorithm),
                Issuer = model.Issuer,
                Audience = model.Issuer
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            string token = jwtSecurityTokenHandler.WriteToken(securityToken);

            return token;
        }

        /// <summary>
        /// Receives the claims of token by given token as string.
        /// </summary>
        /// <remarks>
        /// Pay attention, one the token is FAKE the method will throw an exception.
        /// </remarks>
        /// <param name="token"></param>
        /// <returns>IEnumerable of claims for the given token.</returns>
        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private Methods
        private SecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }
        #endregion
    }
}