using StoreModel.Account;
using System;
using System.Threading.Tasks;

namespace StoreService.Interface
{
    public interface IAccountService
    {
        string LoginSiteUser(UserCredentials userCredentials, bool isAuthenticated);
        string LoginUser(UserCredentials userCredentials, bool isAuthenticated);
        AuthLoginAttempt AttemptLoginUser(UserCredentials userCredentials);
        Users GetLoginUser(UserCredentials userCredentials, AuthLoginAttempt attempt);
        string AuthorizeUserJwt(Users user);
        Task<bool> UpdateUserVisitor(Guid visitorUid, Guid userUid);
        string RegisterUser(UserCredentials userCredentials);
        Task<Guid> GetVisitorUid();
        Users GetUser(Guid userUid);
        Users GetUser(String userEmail);
    }
}
