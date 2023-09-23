using Microsoft.AspNetCore.Identity;

namespace WebAPI_JWT.Repositories
{
    public interface IJwtRepository<T> where T:IdentityUser
    {
        string CreateToken(T user);
    }
}
