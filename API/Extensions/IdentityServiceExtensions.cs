using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServiceExtensions(this IServiceCollection services, IConfiguration config)
        {

            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            }
            )
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleValidator<RoleValidator<AppRole>>()
            .AddEntityFrameworkStores<DContext>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                   ValidateIssuer = false,
                   ValidateAudience = false,
               };
               options.Events=new JwtBearerEvents{
                   OnMessageReceived=context=>{
                       var accessToken =context.Request.Query["access_token"];
                       var path = context.Request.Path;
                       if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")){//from startup
                           context.Token= accessToken;
                       }
                       return Task.CompletedTask;
                   }
               };
           });
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", p => p.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole",p=>p.RequireRole("Moderator","Admin"));
            });
            return services;
        }
    }
}