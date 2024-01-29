using _10_Authen_TrinhCV.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace _10_Authen_TrinhCV.ServicesCommon
{
    public class JwtSecurityService : ISecurityService
    {
        private const string AUTH_ISSUER = "ATC TrinhCV";
        private const string AUTH_AUDIENCE = "https://trinhcv.com";

        private readonly IConfiguration _configuration;
        public JwtSecurityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(string userName, string roleName, string email, int id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = GetSecurityKeyByteArray();
            var now = DateTime.UtcNow;
            const int ExpiresIn = 5;

            // Create the json web token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    // How many hours until the token will expire
                    new Claim("ExpiresIn", ExpiresIn.ToString()),
                    new Claim("ValidTo", now.AddMinutes(ExpiresIn).ToString()),
                    new Claim("Username", userName),
                    new Claim("Role", roleName),
                    new Claim("Id", id.ToString(), ClaimValueTypes.Integer),
                    new Claim("Email", email),
                }),
                Issuer = AUTH_ISSUER,
                Audience = AUTH_AUDIENCE,
                Expires = now.AddMinutes(ExpiresIn),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest)
            };

            // Create the json web token from the descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Create and return the jwt string to the mobile client to be used in subsequent requests
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
        /// <summary>
        /// Validate a json web token (jwt)
        /// Used for DAT
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errMsg">Exception Message</param>
        /// <returns></returns>
        public TokenInfo ValidateToken(string token, out string errMsg)
        {
            errMsg = "";
            TokenInfo tokenInfo;
            try
            {
                SecurityToken securityToken;

                // Validate the jwt. If successful, return the claims.
                var principal = ValidateToken(token, out securityToken);

                tokenInfo = new TokenInfo();

                var tokenClaims = principal.Claims;
                var claims = tokenClaims.ToList();

                // Extract all claims to token info, and convert each claim as necessary
                foreach (var claim in claims)
                {
                    switch (claim.Type)
                    {
                        case "Id":
                            tokenInfo.Id = claim.Value;
                            break;
                        case "Username":
                            tokenInfo.Username = claim.Value;
                            break;
                        case "Role":
                            tokenInfo.Role = claim.Value;
                            break;
                        case "Email":
                            tokenInfo.Email = claim.Value;
                            break;
                        case "ValidTo":
                            tokenInfo.ValidTo = claim.Value;
                            break;
                        default:
                            // ignore
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                errMsg = exception.Message;
                throw (exception);
            }

            return tokenInfo;
        }
        public ClaimsPrincipal ValidateToken(string token, out SecurityToken validatedToken)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken incomingToken = jwtSecurityTokenHandler.ReadJwtToken(token);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = AUTH_ISSUER,
                ValidateAudience = true,
                ValidAudience = AUTH_AUDIENCE,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                // This is to handle any differences in time between the authorization server and the API
                ClockSkew = new TimeSpan(0, 0, 30)
            };

            ClaimsPrincipal claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            return claimsPrincipal;
        }
        private byte[] GetSecurityKeyByteArray()
        {
            var jwtConstant = _configuration["Parameters:Jwtkey"];
            var bytes = new byte[jwtConstant.Length * sizeof(char)];
            Buffer.BlockCopy(jwtConstant.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
