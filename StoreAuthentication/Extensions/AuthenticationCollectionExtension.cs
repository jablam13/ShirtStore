using StoreAuthentication.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace StoreAuthentication.Extensions
{
    public static class AuthenticationCollectionExtension
    {
        public static IServiceCollection AddJwtAuthenticationWithProtectedCookie(this IServiceCollection services,
            TokenValidationParameters tokenValidationParams,
            string authenticationScheme,
            string applicationDiscriminator = null,
            AuthUrlOptions authUrlOptions = null)
        {
            if (tokenValidationParams == null)
            {
                throw new ArgumentNullException(
                    $"{nameof(tokenValidationParams)} is a required parameter. " +
                    $"Please make sure you've provided a valid instance with the appropriate values configured.");
            }

            if (string.IsNullOrEmpty(authenticationScheme) || string.IsNullOrWhiteSpace(authenticationScheme))
            {
                authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }

            var hostingEnvironment = services.BuildServiceProvider().GetService<IHostingEnvironment>();

            var appDiscriminator = $"{applicationDiscriminator ?? hostingEnvironment.ApplicationName}";
            services.AddDataProtection(options => options.ApplicationDiscriminator = appDiscriminator)
                .SetApplicationName(appDiscriminator);
            services.AddScoped<IDataSerializer<AuthenticationTicket>, TicketSerializer>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(serviceProvider =>
                new JwtTokenGenerator(tokenValidationParams.ToTokenOptions(1)));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = authenticationScheme;
                options.DefaultSignInScheme = authenticationScheme;
                options.DefaultChallengeScheme = authenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Expiration = TimeSpan.FromMinutes(1);
                options.TicketDataFormat = new JwtAuthTicketFormat(tokenValidationParams,
                    services.BuildServiceProvider().GetService<IDataSerializer<AuthenticationTicket>>(),
                    services.BuildServiceProvider().GetDataProtector(new[]
                    {
                        $"{applicationDiscriminator ?? hostingEnvironment.ApplicationName}-Auth1"
                    }));

                options.LoginPath = authUrlOptions != null ?
                    new PathString(authUrlOptions.LoginPath)
                    : new PathString("/Account/Login");
                options.LogoutPath = authUrlOptions != null ?
                    new PathString(authUrlOptions.LogoutPath)
                    : new PathString("/Account/Logout");
                options.AccessDeniedPath = options.LoginPath;
                options.ReturnUrlParameter = authUrlOptions?.ReturnUrlParameter ?? "returnUrl";
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = tokenValidationParams;
            });

            return services;
        }
    }

    /// <summary>
    /// A simple structure to store the configured login/logout paths and the name of the return url parameter
    /// </summary>
    public sealed class AuthUrlOptions
    {
        /// <summary>
        /// The login path to redirect the user to incase of unauthenticated requests.
        /// Default is "/Account/Login"
        /// </summary>
        public string LoginPath { get; set; }

        /// <summary>
        /// The path to redirect the user to once they have logged out.
        /// Default is "/Account/Logout"
        /// </summary>
        public string LogoutPath { get; set; }

        /// <summary>
        /// The path to redirect the user to following a successful authentication attempt.
        /// Default is "returnUrl"
        /// </summary>
        public string ReturnUrlParameter { get; set; }
    }
}
