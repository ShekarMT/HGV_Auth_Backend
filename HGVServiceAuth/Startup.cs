using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HGVServiceAuth.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HGVServiceAuth
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
            services.AddCors(options =>
            {
                options.AddPolicy("EnableCors", builder =>
                 {
                     builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                 });
            });


            services.AddAuthentication(cfg =>
            {
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = ConfigurationReader.AppSetting["Authentication:AzureAd:AADInstance"] + ConfigurationReader.AppSetting["Authentication:AzureAd:TenantId"]; //pass tenant ID here
                options.Audience = ConfigurationReader.AppSetting["Authentication:AzureAd:Audience"];
                options.Events = new JwtBearerEvents()
                {

                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = ConfigurationReader.AppSetting["Authentication:AzureAd:TenantId"],
                    ValidateAudience = true,
                    ValidAudience = ConfigurationReader.AppSetting["Authentication:AzureAd:Audience"]
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OnlyInventoryManagerAccess", policy => policy.RequireRole("InventoryManager"));
                options.AddPolicy("OnlyPremiumSpecialistAccess", policy => policy.RequireRole("PremiumSpecialist"));

            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("EnableCors");
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
