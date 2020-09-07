using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Helpers;
using WebApi.IServices;
using WebApi.Models;
using WebApi.Services;
using Role = WebApi.Models.Role;

namespace WebApi
{
    /// <summary>
    /// Configures services and the application request pipeline.
    /// </summary>
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private AppSettings _appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="env">The current web host environment.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Used to specify the contract for a collection of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole",
                    policy => policy.RequireRole(Models.EnumerationTypes.Role.Admin.ToString()));
            });

            services.AddDbContext<AppDbContext>();

            services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

            services.AddCors();
            services
                .AddControllers()
                .AddFluentValidation(configuration =>
                {
                    configuration.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });

            // Configure strongly typed settings objects.
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            _appSettings = appSettingsSection.Get<AppSettings>();

            // Configure JWT authentication.
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            services
                .AddAuthentication(configuration =>
                {
                    configuration.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    configuration.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configuration =>
                {
                    configuration.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                            var userId = context.Principal.Identity.Name;
                            if (userId == null) context.Fail("Unauthorized");

                            var user = db.Users.AsNoTracking().Include(x => x.Role)
                                .FirstOrDefault(x => x.Id == Guid.Parse(userId));

                            if (user != null)
                            {
                                var identity = context.Principal.Identity as ClaimsIdentity;
                                identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.Name));
                            }
                            else
                            {
                                context.Fail("Unauthorized");
                            }

                            return Task.CompletedTask;
                        }
                    };
                    configuration.RequireHttpsMetadata = false;
                    configuration.SaveToken = true;
                    configuration.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // Add Swagger.
            services.AddSwaggerGen(configuration =>
            {
                configuration.SwaggerDoc("v1", new OpenApiInfo {Title = "Web API", Version = "v1"});
                const string xmlFile = "ClassLibrary.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                configuration.IncludeXmlComments(xmlPath);
                configuration.CustomSchemaIds(type => type.ToString());
                configuration.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                        "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                configuration.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });
            });

            services.AddLocalization();

            // Configure DI for application services.
            // Transient objects are always different; a new instance is provided to every controller and every service.
            // Scoped objects are the same within a request, but different across different requests.
            // Singleton objects are the same for every object and every request.
            services.AddScoped<IPasswordHelper, PasswordHelper>();
            services.AddScoped<IAuthHelper, AuthHelper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.
        /// </param>
        /// <param name="appDbContext">The database context.</param>
        /// <param name="passwordHelper">The helper object for working with passwords.</param>
        /// <param name="logger">The logger.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext appDbContext,
            IPasswordHelper passwordHelper, ILogger<Startup> logger)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            // Handle all uncaught exceptions. 
            app.UseExceptionHandler(a => a.Run(
                async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;
                    logger.LogError(exception, $"Unhandled exception caught by middleware: {exception.Message}");
                    var result = JsonConvert.SerializeObject(new {error = exception.Message});
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync(result);
                }));

            // Migrate any database changes on startup (includes initial database creation).
            appDbContext.Database.Migrate();

            // Seed admin user and role.
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<AppDbContext>();
                if (db.Users.FirstOrDefault(x => x.Username == _appSettings.AdminUsername) == null)
                {
                    var (passwordHash, passwordSalt) = passwordHelper.CreateHash(_appSettings.AdminPassword);

                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Username = _appSettings.AdminUsername,
                        Email = _appSettings.AdminEmail,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    var role = new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = Models.EnumerationTypes.Role.Admin.ToString(),
                        CreatedById = user.Id
                    };

                    db.Roles.Add(role);
                    user.RoleId = role.Id;

                    db.SaveChanges();
                }
            }

            // The localization middleware must be configured before
            // any middleware which might check the request culture.
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("bs")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseRouting();

            // Set global CORS policy.
            app.UseCors(configuration => configuration.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/health");
                }
            );

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
                c.DisplayRequestDuration();
            });
        }
    }
}