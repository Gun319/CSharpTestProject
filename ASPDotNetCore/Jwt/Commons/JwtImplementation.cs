using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt.Commons
{
    /// <summary>
    /// jwt实现类
    /// </summary>
    public class JwtImplementation
    {
        private readonly IConfiguration _configuration;

        public JwtImplementation(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userRole"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public string CreateToken(string userName, List<string> userRole, List<string> permissions)
        {
            // 定义需要使用到的 Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };

            foreach (var role in userRole)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            // 读取 SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]!));

            // 选择加密算法
            var encryption = SecurityAlgorithms.HmacSha256;

            // 生成 Credentials
            var signingCredentials = new SigningCredentials(secretKey, encryption);

            // 生成 Token
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"], // Issuer
                _configuration["Authentication:Audience"], // Audience
                claims, //Claims
                DateTime.Now, // notBefore
                DateTime.Now.AddSeconds(30), // expires
                signingCredentials // Credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
