using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
                    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;


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
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                })
            .AddGoogle(options =>
             {
                 options.ClientId = configuration["Authentication:Google:ClientId"];
                 options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                 options.SignInScheme = IdentityConstants.ExternalScheme;

                 // Optional: Map specific claims from Google
                 options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                 options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                 options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
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
