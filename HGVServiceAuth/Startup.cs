using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
                options.Authority = "https://login.microsoftonline.com/41814658-1358-4ce7-b93d-bc3fce646e07";
                //Configuration["AppSettings:Authentication:AzureAd:AADInstance"]+Configuration["AppSettings:Authentication:AzureAd:TenantId"];
                options.Audience = "d1cb9241-6542-4627-a9fc-2958c0bfc6af";
                //Configuration["AppSettings:Authentication:AzureAd:ClientId"];
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
                    ValidIssuer = "41814658-1358-4ce7-b93d-bc3fce646e07",
                    ValidateAudience = true,
                    ValidAudience = "d1cb9241-6542-4627-a9fc-2958c0bfc6af",
                    //IssuerSigningKey = new RsaSecurityKey(new RSACryptoServiceProvider(2048).ExportParameters(true))
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("jEJb19FoczzPzNTvwW2q7HzY847geIYVUkYqY5r9+fk="))
                };
                //options.SaveToken = true;

            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OnlyAdminAccess", policy => policy.RequireRole("Admin"));
                options.AddPolicy("OnlyUserAccess", policy => policy.RequireRole("User"));

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

            //app.UseJwtBearerAuthentication(new JwtBearerOptions
            //{
            //    Authority = Configuration["AppSettings:Authentication:AzureAd:AADInstance"]
            //    + Configuration["AppSettings:Authentication:AzureAd:TenantId"],
            //    Audience = Configuration["AppSettings:Authentication:AzureAD:ClientId"],
            //    TokenValidationParameters =
            //    new TokenValidationParameters
            //    {
            //        ValidIssuer =
            //        Configuration["AppSettings:Authentication:AzureAd:AADInstance"]
            //      + Configuration["AppSettings:Authentication:AzureAd:TenantId"] + "/ v2.0"
            //    }
            //});

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
