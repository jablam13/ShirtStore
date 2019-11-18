using StoreModel.Account;
using System;

namespace StoreService.Interface
{
    public interface IAccountService
    {
        string LoginSiteUser(UserCredentials userCredentials, bool isAuthenticated);
        string LoginUser(UserCredentials userCredentials, bool isAuthenticated);
        string RegisterUser(UserCredentials userCredentials);
        string RegisterUser(Users user, string password);
        Guid GetVisitorUid();
        Users GetUser(Guid userUid);
        Users GetUser(String userEmail);
    }
}
