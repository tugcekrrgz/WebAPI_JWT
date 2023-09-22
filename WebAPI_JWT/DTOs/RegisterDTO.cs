
using System.ComponentModel.DataAnnotations;

namespace WebAPI_JWT.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage ="Kullanıcı Adı Boş Geçilemez!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email Geçilemez!")]
        [EmailAddress(ErrorMessage ="Email Formatını Doğru Giriniz!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Boş Geçilemez!")]
        public string Password { get; set; }
    }
}
