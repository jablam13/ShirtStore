using StoreModel.Account;
using System;
using System.Threading.Tasks;

namespace StoreService.Interface
{
    public interface IAccountService
    {
        string LoginSiteUser(UserCredentials userCredentials, bool isAuthenticated);
        string LoginUser(UserCredentials userCredentials, bool isAuthenticated);
        string RegisterUser(UserCredentials userCredentials);
        string RegisterUser(Users user, string password);
        Task<Guid> GetVisitorUid();
        Users GetUser(Guid userUid);
        Users GetUser(String userEmail);
    }
}
