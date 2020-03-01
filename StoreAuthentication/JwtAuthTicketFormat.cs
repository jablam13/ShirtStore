using StoreAuthentication.Types;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace StoreAuthentication
{
    public sealed class JwtAuthTicketFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string Algorithm = SecurityAlgorithms.HmacSha256;
        private readonly TokenValidationParameters validationParameters;
        private readonly IDataSerializer<AuthenticationTicket> ticketSerializer;
        private readonly IDataProtector dataProtector;

        public JwtAuthTicketFormat(TokenValidationParameters validationParameters,
            IDataSerializer<AuthenticationTicket> ticketSerializer,
            IDataProtector dataProtector)
        {
            this.validationParameters = validationParameters ??
                throw new ArgumentNullException($"{nameof(validationParameters)} cannot be null");
            this.ticketSerializer = ticketSerializer ??
                throw new ArgumentNullException($"{nameof(ticketSerializer)} cannot be null");
            this.dataProtector = dataProtector ??
                throw new ArgumentNullException($"{nameof(dataProtector)} cannot be null");
        }

        public AuthenticationTicket Unprotect(string protectedText)
            => Unprotect(protectedText, null);

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            var authTicket = ticketSerializer.Deserialize(
                dataProtector.Unprotect(
                    Base64UrlTextEncoder.Decode(protectedText)));

            var embeddedJwt = authTicket
                .Properties?
                .GetTokenValue(TokenConstants.TokenName);

            SecurityToken token = null;

            try
            {
                new JwtSecurityTokenHandler()
                    .ValidateToken(embeddedJwt, validationParameters, out token);

                if (!(token is JwtSecurityToken jwt))
                {
                    throw new SecurityTokenValidationException("JWT token was found to be invalid");
                }

                if (!jwt.Header.Alg.Equals(Algorithm, StringComparison.Ordinal))
                {
                    throw new ArgumentException($"Algorithm must be '{Algorithm}'");
                }
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }

            return authTicket;
        }

        public string Protect(AuthenticationTicket data) => Protect(data, null);

        public string Protect(AuthenticationTicket data, string purpose)
        {
            var array = ticketSerializer.Serialize(data);

            return Base64UrlTextEncoder.Encode(dataProtector.Protect(array));
        }
    }
}
