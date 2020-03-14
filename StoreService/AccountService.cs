using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using StoreAuthentication.Abstractions;
using StoreModel.Account;
using StoreRepository.Interface;
using StoreService.Interface;
using StoreService.Utility;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StoreService
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IHttpContextAccessor context;
        private readonly IUsersRepository userRep;
        private readonly IJwtTokenGenerator tokenGenerator;

        public AccountService(
            IHttpContextAccessor httpContextAccessor,
            IUsersRepository _userRep,
            IJwtTokenGenerator _tokenGenerator)
        {
            context = httpContextAccessor;
            userRep = _userRep;
            tokenGenerator = _tokenGenerator;
        }
        public string LoginSiteUser(UserCredentials userCredentials, bool isAuthenticated)
        {
            var ifNotNull = !(string.IsNullOrWhiteSpace(userCredentials.EmailAddress) || string.IsNullOrWhiteSpace(userCredentials.Password));
            var accessToken = "";
            if (isAuthenticated && !ifNotNull)
            {
                return accessToken;
            }

            //get user validation info
            var userAuth = userRep.GetParticipantByUsername(userCredentials.EmailAddress);

            //validate user
            bool validAuthentication = CheckAuthentication(userAuth, userCredentials);

            if (validAuthentication)
            {
                var user = userRep.GetUserByEmail(userCredentials.EmailAddress);

                var accessTokenResult = tokenGenerator.GenerateAccessTokenWithClaimsPrincipal(
                    userCredentials.EmailAddress,
                    AddMyClaims(user));
                context.HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal,
                    accessTokenResult.AuthProperties);

                accessToken = accessTokenResult?.AccessToken?.ToString() ?? "";
            }

            return accessToken;
        }

        public string LoginUser(UserCredentials userCredentials, bool isUserAuthenticated)
        {
            var ifNotNull = !(string.IsNullOrWhiteSpace(userCredentials.EmailAddress) || string.IsNullOrWhiteSpace(userCredentials.Password));
            var accessToken = "";
            if (isUserAuthenticated && !ifNotNull)
            {
                return accessToken;
            }

            //get user validation info
            var userAuth = userRep.GetParticipantByUsername(userCredentials.EmailAddress);

            //validate user
            bool validAuthentication = CheckAuthentication(userAuth, userCredentials);

            if (validAuthentication)
            {
                var user = userRep.GetUserByEmail(userCredentials.EmailAddress);

                var accessTokenResult = tokenGenerator.GenerateAccessTokenWithClaimsPrincipal(
                    userCredentials.EmailAddress,
                    AddMyClaims(user));
                context.HttpContext.SignInAsync(
                    accessTokenResult.ClaimsPrincipal,
                    accessTokenResult.AuthProperties);

                accessToken = accessTokenResult?.AccessToken?.ToString() ?? "";
                return accessToken;
            }

            return accessToken;
        }

        public AuthLoginAttempt AttemptLoginUser(UserCredentials userCredentials)
        {
            var paramsAreNull = string.IsNullOrWhiteSpace(userCredentials.EmailAddress) || string.IsNullOrWhiteSpace(userCredentials.Password);

            if (paramsAreNull)
            {
                throw new ArgumentNullException();
            }

            try
            {
                return userRep.GetParticipantByUsername(userCredentials.EmailAddress);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public Users GetLoginUser(UserCredentials userCredentials, AuthLoginAttempt attempt)
        {
            //validate user
            bool validAuthentication = CheckAuthentication(attempt, userCredentials);

            if (!validAuthentication)
            {
                throw new AuthenticationException();
            }

            try
            {
                return userRep.GetUserByEmail(userCredentials.EmailAddress);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string AuthorizeUserJwt(Users user)
        {
            try
            {
                var userClaims = AddMyClaims(user);
                var accessTokenResult = tokenGenerator
                    .GenerateAccessTokenWithClaimsPrincipal(user.EmailAddress, userClaims);

                return accessTokenResult?.AccessToken?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string RegisterUser(UserCredentials userCredentials)
        {
            var accessToken = "";

            //check if userCredentials arent null 
            if (!CheckUserCredentials(userCredentials))
            {
                return accessToken;
            }

            //check if username exists
            if (userRep.UsernameExists(userCredentials.EmailAddress))
            {
                return accessToken;
            }

            //convert UserCredentials to Users object 
            var user = ConvertToUser(userCredentials);

            //insert user into database, update user with id, uid, etc..
            user = userRep.Register(user);

            if (user != null)
            {
                //generate token for user
                var accessTokenResult = tokenGenerator.GenerateAccessTokenWithClaimsPrincipal(
                            user.EmailAddress,
                            AddMyClaims(user));

                //sign user in
                context.HttpContext.SignInAsync(accessTokenResult.ClaimsPrincipal,
                    accessTokenResult.AuthProperties);

                accessToken = accessTokenResult?.AccessToken?.ToString() ?? "";
            }

            return accessToken;
        }

        public async Task<bool> UpdateUserVisitor(Guid visitorUid, Guid userUid)
        {
            bool success;
            try
            {
                success = await userRep.UpdateUserVisitor(userUid, visitorUid).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }

            return success;
        }
        public async Task<Guid> GetVisitorUid()
        {
            Guid visitorUid;
            try
            {
                visitorUid = await userRep.GetVisitorUid();
            }
            catch (Exception)
            {
                throw;
            }

            return visitorUid;
        }

        public Users GetUser(Guid userUid)
        {
            var user = userRep.GetUserByUid(userUid);
            return user;
        }

        public Users GetUser(string userEmail)
        {
            var user = userRep.GetUserByEmail(userEmail);
            return user;
        }

        private static IEnumerable<Claim> AddMyClaims(Users user)
        {
            var myClaims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Uid.ToString()),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim("FirstName", user.FirstName ?? ""),
                new Claim("LastName", user.LastName ?? ""),
        };

            return myClaims;
        }

        private static bool CheckAuthentication(AuthLoginAttempt authLoginAttempt, UserCredentials userCredentials)
        {
            bool validHash = false;
            bool isNotNull = authLoginAttempt != null && !string.IsNullOrWhiteSpace(userCredentials.Password);
            if (isNotNull && authLoginAttempt.Salt != null)
                validHash = authLoginAttempt.Hash == PasswordUtilities.GenerateHash(userCredentials.Password, authLoginAttempt.Salt);
            return isNotNull && validHash;
        }

        private static bool CheckUserCredentials(UserCredentials user)
        {
            return user != null
                && !string.IsNullOrWhiteSpace(user?.EmailAddress)
                && !string.IsNullOrWhiteSpace(user?.Password);
        }

        private static Users ConvertToUser(UserCredentials userCredentials)
        {
            var salt = PasswordUtilities.GenerateSalt();
            var hash = PasswordUtilities.GenerateHash(userCredentials.Password, salt);

            var user = new Users()
            {
                Username = userCredentials.EmailAddress ?? "",
                EmailAddress = userCredentials.EmailAddress ?? "",
                Salt = salt,
                Hash = hash,
                FirstName = userCredentials.FirstName ?? "",
                LastName = userCredentials.LastName ?? ""
            };

            return user;
        }
    }
}
