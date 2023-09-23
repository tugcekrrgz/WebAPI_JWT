using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_JWT.DTOs;

namespace WebAPI_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName=registerDTO.Username, 
                    Email=registerDTO.Email
                };

                var result=await _userManager.CreateAsync(user,registerDTO.Password);

                if(result.Succeeded) 
                {
                    return Ok("Kayıt Başarılı!");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code,error.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return Ok(registerDTO);    
            }
            
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if(ModelState.IsValid) 
            {
                var user=await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    return NotFound("Kullanıcı Bulunamadı!");
                }
                var result=_userManager.CheckPasswordAsync(user, loginDTO.Password).Result;

                if (result)
                {
                    //token
                    var token = CreateToken(user);
                    return Ok(token);
                }

                return BadRequest("Hatalı İstek!");
            }
            else
            {
                return BadRequest("Bilgiler Hatalı!");
            }
        }


        public string CreateToken(IdentityUser user)
        {
            //Kullanıcı Bilgilerine Ait Token Oluşturma

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
                claims:claims,
                expires:expire,
                signingCredentials:cred
                );

            var result=new JwtSecurityTokenHandler().WriteToken(token);

            return result;
        }


    }
}
