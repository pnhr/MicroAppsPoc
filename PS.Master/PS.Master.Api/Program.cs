using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PS.Master.Api.Auth;
using PS.Master.Api.AutoMapperProfiles;
using PS.Master.Api.Services.Definitions;
using PS.Master.Api.Services.Interfaces;
using PS.Master.Data;
using PS.Master.Data.Database;
using PS.Master.Data.Definitions;
using PS.Master.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;

namespace PS.Master.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connStr = builder.Configuration.GetConnectionString("AppDbConnection");
            string LogConnStr = builder.Configuration.GetConnectionString("AppLogDbConnection");

            string masterDbConnStr = builder.Configuration.GetConnectionString("AppMasterDbConnection");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(connStr));
            builder.Services.AddDbContextPool<AppMasterDbContext>(options => options.UseSqlServer(masterDbConnStr));


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
            builder.Services.AddScoped<IAppFileService, AppFileService>();
            builder.Services.AddScoped<IAppManagerService, AppManagerService>();


            builder.Services.AddScoped<ISampleRepository, SampleRepository>();
            builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
            builder.Services.AddScoped<IAppManagerRepository, AppManagerRepository>();
            builder.Services.AddScoped<IAdoRepo, AdoRepo>();

            builder.Services.AddAutoMapper(typeof(EmployeeAutoMapperProfile));


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCustomJwtBearer(builder.Configuration["Authentication:JWTSettings:SecretKey"]);

            //builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

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

            app.UseWebAssemblyDebugging();
            app.UseSwagger();
            app.UseSwaggerUI();

            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseWebAssemblyDebugging();
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    app.UseHsts();
            //}

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