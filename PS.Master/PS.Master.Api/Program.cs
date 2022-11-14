using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PS.Master.Api.Auth;
using PS.Master.Api.AutoMapperProfiles;
using PS.Master.Api.Services.Definitions;
using PS.Master.Api.Services.Interfaces;
using PS.Master.Data;
using PS.Master.Data.Database;
using PS.Master.Data.Definitions;
using PS.Master.Logging;
using System.Text;

namespace PS.Master.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connStr = builder.Configuration.GetConnectionString("AppDbConnection");
            string LogConnStr = builder.Configuration.GetConnectionString("AppLogDbConnection");

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(connStr));
            builder.Logging.AddDbLogger(config =>
            {
                config.ConnectionString = LogConnStr;
                config.LogLevel = new List<LogLevel>();
                config.LogLevel.Add(LogLevel.Warning);
                config.LogLevel.Add(LogLevel.Error);
                config.LogLevel.Add(LogLevel.Critical);
            });

            builder.Services.AddScoped<LogAttribute>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISampleService, SampleService>();
            builder.Services.AddScoped<IAppManagerService, AppManagerService>();
            builder.Services.AddScoped<ISampleRepository, SampleRepository>();

            builder.Services.AddAutoMapper(typeof(EmployeeAutoMapperProfile));


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCustomJwtBearer(builder.Configuration["Authentication:JWTSettings:SecretKey"]);

            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test01", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."

                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });

            });
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapRazorPages();
            app.MapControllers();

            app.UseMiddleware(typeof(ExceptionHandlerMiddleware));
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}