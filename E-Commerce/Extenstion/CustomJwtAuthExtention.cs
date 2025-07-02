using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Extenstion
{
   
        public static class CustomJwtAuthExtention
        {
            public static void AddCustomJwtAuth(this IServiceCollection services, ConfigurationManager configuration)
            {
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

                }).AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                        ValidateLifetime = true,
                        RoleClaimType = ClaimTypes.Role
                    };
                });
            }
        public static void AddSwaggerGenJWTAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "E-Commerce-API",
                    Description = "the right Markets",
                    Contact = new OpenApiContact()
                    {
                        Email = "tarqe2025@gmail.com",
                        Name = "tareq Alnaqeeb"

                    }

                });

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Key Here ?"
                });

                o.AddSecurityRequirement(new OpenApiSecurityRequirement() { 
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Name = "Bearer",
                    In = ParameterLocation.Header
                    },
                    new List<string>()
                }

                });
            });

        }
        }
    
}
