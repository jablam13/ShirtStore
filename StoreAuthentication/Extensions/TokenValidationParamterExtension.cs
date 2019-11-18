using StoreAuthentication.Types;
using Microsoft.IdentityModel.Tokens;

namespace StoreAuthentication.Extensions
{
    public static class TokenValidationParameterExtensions
    {
        public static TokenOption ToTokenOptions(this TokenValidationParameters tokenValidationParameters,
            int tokenExpiryInMinutes = 5)
        {
            return new TokenOption(tokenValidationParameters.ValidIssuer,
                tokenValidationParameters.ValidAudience,
                tokenValidationParameters.IssuerSigningKey,
                tokenExpiryInMinutes);
        }
    }
}
