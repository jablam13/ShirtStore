using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using StoreModel.Generic;
using StoreRepository;
using StoreService;
using StoreAuthentication.Extensions;
using PaymentService.Braintree;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Stripe;

namespace ShirtStoreService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(appSettings =>
            {
                appSettings.SiteId = Configuration.GetValue<int>("SiteId");
                appSettings.AnalyticsEnabled = Configuration.GetValue<bool>("AppSettings:AnalyticsEnabled");
                appSettings.ConnectionString = Configuration.GetValue<string>("DataInfo:ConnectionString");
                appSettings.SendGridSetting = Configuration.GetSection("SendGridSettings").Get<SendGridSettings>();
                appSettings.BraintreeSettings = Configuration.GetSection("BraintreeSettings").Get<BraintreeSettings>();
            });

            services.AddRepositoryCollection(Configuration);
            services.AddServiceCollection(Configuration);
            services.AddBraintree(Configuration);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            // retrieve the configured token params and establish a TokenValidationParameters object,
            // we are going to need this later.
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Token:SigningKey"]));
            var validationParams = new TokenValidationParameters()
            {
                ClockSkew = TimeSpan.Zero,

                ValidateAudience = true,
                ValidAudience = Configuration["Token:Audience"],

                ValidateIssuer = true,
                ValidIssuer = Configuration["Token:Issuer"],

                IssuerSigningKey = signingKey,
                ValidateIssuerSigningKey = true,

                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            //Use CookieAuthenticationDefaults.AuthenticationScheme for cookie generation
            //Use JwtBearerDefaults.AuthenticationScheme for passing string token and handling token passed manually from header
            //services.AddJwtAuthenticationWithProtectedCookie(validationParams, JwtBearerDefaults.AuthenticationScheme);
            services.AddJwtAuthenticationWithDataProtection(validationParams, JwtBearerDefaults.AuthenticationScheme);

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            var exposedHeaders = new string[] { "test", "visitor" };
            services.AddCors(o => o.AddPolicy("DevPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                       .AllowAnyMethod()
                       .AllowAnyHeader();
                       //.WithExposedHeaders(exposedHeaders);
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("DevPolicy");
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /// Add header:
            app.Use((context, next) =>
            {
                context.Response.Headers.Add("test","test-header");
                return next.Invoke();
            });
        }
    }
}
