using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_JWT.Models.Context
{
    public class ProjectContext:IdentityDbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options):base(options)
        {

        }
    }
}
