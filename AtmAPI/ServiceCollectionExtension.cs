using Application.ConfigSettings;
using Application.DTOs;
using Application.Mappings;
using DataAccess;
using Domain.Entities;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data;
using Polly;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace AtmAPI
{
    public static class ServiceCollectionExtension
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(DtoToDomainProfile));

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                     new HeaderApiVersionReader("x-api-version"),
                     new MediaTypeApiVersionReader("x-api-version")
                );
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
            services.ConfigureOptions<SwaggerConfigureOptions>();

            //---------------------------- CORS setting---------------------------------------------------------//
            services.AddCors(options =>
            {
                options.AddPolicy("CORS",
                    builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
                , options => options.EnableRetryOnFailure(
                  maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null)));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().
            AddDefaultTokenProviders();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            //------------------------------------JWT Authentication Settings--------------------------------------//
            var appsettings = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(appsettings);
            var appSettingValues = appsettings.Get<JwtSettings>();

            //Encoding The Secret
            var key = Encoding.ASCII.GetBytes(appSettingValues.Secret);


            //------------------------------------- Authentication Middleware ---------------------------------------//            

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                LifetimeValidator = TokenLifetimeValidator.Validate,
                ValidIssuer = appSettingValues.Site,
                ValidAudience = appSettingValues.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
                options.Events = new JwtBearerEvents();
                options.Events.OnChallenge = context =>
                {
                    // Skip the default logic.
                    context.HandleResponse();

                    var payload = BaseResponse.Failure("01", "Unauthorised");
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 401;

                    return context.Response.WriteAsync(payload.ToString());
                };
            });

            // --------------------------------DevApi Config ---------------------------------------//
            var devAPI = configuration.GetSection("DevApiSettings");
            services.Configure<DevApiSettings>(devAPI);
            var devApiValues = devAPI.Get<DevApiSettings>();

            

            // ----------------------------------------Email Config--------------------------------------------//

            

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

            //----------------------------------------  Configuration Settings -------------------------------------------------//

            services.Configure<AppEndpointSettings>(configuration.GetSection("AppEndpointSettings")); 

        }
    }
}

public static class TokenLifetimeValidator
{
    public static bool Validate(
        DateTime? notBefore,
        DateTime? expires,
        SecurityToken tokenToValidate,
        TokenValidationParameters @param
    )
    {
        return (expires != null && expires > DateTime.UtcNow);
    }
}
