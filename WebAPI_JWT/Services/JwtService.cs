using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_JWT.Repositories;

namespace WebAPI_JWT.Services
{
    public class JwtService : IJwtRepository<IdentityUser>
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(IdentityUser user)
        {
            // Kullanıcı Bilgilerine Ait Token Oluşturma

            //Claims
            var claims = new List<Claim>()
            {
                //Subject : Claim JWT'nin subject alanını temsil eder.
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),

                //JWT Id : Claim JTI alanını temsil eder. Rastgele bir Guid değeri içerir. JWTI alanı o JWT'nin benzersiz bir kimliğini temsil etmektedir. JWT tekrar kullanılabilmesi ve güvenliği için geçerlidir.
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

                //Name Identifier : Claim JWT'nin nameidentifier alanını temsil eder ve kulalanıcının kimlik bilgisini içerir.
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };

            //Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            //Credentials
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Expire
            var expire = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JWT:ExpireDays"]));

            //Token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: expire,
                signingCredentials: cred
                );

            var result = new JwtSecurityTokenHandler().WriteToken(token);

            return result;
        }
    }
}
