using BuildUp.API.Services;
using BuildUp.API.Services.Interfaces;
using BuildUp.API.Settings;
using BuildUp.API.Settings.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuildUp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Buildup API",
                    Description = "The API for the Buildup Program of New Talents",
                    Contact = new OpenApiContact
                    {
                        Name = "Victor (Feldrise) DENIS",
                        Email = "contact@feldrise.com",
                        Url = new Uri("https://feldrise.com")
                    }
                });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "BuildUp.API.xml");
                c.IncludeXmlComments(filePath);
            });

            // Settings
            services.Configure<MongoSettings>(Configuration.GetSection(nameof(MongoSettings)));
            services.Configure<BuildupSettings>(Configuration.GetSection(nameof(BuildupSettings)));
            services.Configure<OvhMailCredentials>(Configuration.GetSection(nameof(OvhMailCredentials)));
            
            services.AddSingleton<IMongoSettings>(sp => sp.GetRequiredService<IOptions<MongoSettings>>().Value);
            services.AddSingleton<IBuildupSettings>(sp => sp.GetRequiredService<IOptions<BuildupSettings>>().Value);
            services.AddSingleton<IMailCredentials>(sp => sp.GetRequiredService<IOptions<OvhMailCredentials>>().Value);

            // JWT Authentication 
            var buildupSettings = Configuration.GetSection(nameof(BuildupSettings)).Get<BuildupSettings>();
            var key = Encoding.ASCII.GetBytes(buildupSettings.ApiSecret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBuildersService, BuildersService>();
            services.AddScoped<ICoachsService, CoachsService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IBuildOnsService, BuildOnsService>();

            services.AddScoped<IFormsService, FormsService>();
            services.AddScoped<IFilesService, FilesService>();
            services.AddScoped<IProjectsService, ProjectsService>();

            // Other
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BuildUp API");
                c.InjectStylesheet("/swagger/themes/theme-material.css");
                c.InjectJavascript("/swagger/custom-script.js", "text/javascript");
                c.RoutePrefix = "documentation";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
