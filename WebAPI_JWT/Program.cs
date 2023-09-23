using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI_JWT.Models.Context;
using WebAPI_JWT.Repositories;
using WebAPI_JWT.Services;

var builder = WebApplication.CreateBuilder(args);

//API
builder.Services.AddControllers();

builder.Services.AddDbContext<ProjectContext>(options => options.UseSqlServer("server=DESKTOP-I9V1GOS\\SQLEXPRESS;database=SampleJWT;uid=sa;pwd=Tugce8417058;TrustServerCertificate=True"));

//Identity Service
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ProjectContext>();

//Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    //Token doðrulayýcý
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //Issuer : Yayýncý
        ValidateIssuer=true,

        //Audience : Alýcý(kullanýcý)
        ValidateAudience=true,

        //Key : Anahtar Deðer
        ValidateIssuerSigningKey=true,

        //Geçerli Yayýncý
        ValidIssuer = builder.Configuration["JWT:Issuer"],

        //Geçerli Kullanýcý
        ValidAudience = builder.Configuration["JWT:Audience"],
        
        //Yayýncý Giriþ Anahtarý
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});

//IJwtRepository
builder.Services.AddSingleton<IJwtRepository<IdentityUser>, JwtService>();


//Cors
builder.Services.AddCors(cors =>
{
    cors.AddPolicy("JwtCors", options =>
    {
        options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});


// Add services to the container.

var app = builder.Build();


app.UseHttpsRedirection();


app.UseAuthentication();



app.UseRouting();

app.UseCors("JwtCors");

app.UseAuthorization();


//api/[controller]
app.MapControllers();


app.Run();


