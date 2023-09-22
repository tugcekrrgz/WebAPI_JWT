using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI_JWT.DTOs;

namespace WebAPI_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                    var token = CreateToken();
                    return Ok(token);
                }

                return BadRequest("Hatalı İstek!");
            }
            else
            {
                return BadRequest("Bilgiler Hatalı!");
            }
        }


        public string CreateToken()
        {
            //Kullanıcı Bilgilerine Ait Token Oluşturma

            //Claims
            return "test";
        }


    }
}
