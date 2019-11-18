using StoreModel.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreRepository.Interface
{
    public interface IUsersRepository
    {
        Users GetUserByUid(Guid userUid);
        Users GetUserByEmail(string email);
        Users Register(Users user);
        bool UsernameExists(string email);
        Guid? GeneratePasswordResetValidator(string email);
        Users ValidateEmail(Guid guid);
        AuthLoginAttempt GetParticipantByUsername(string email);
        bool LogSuccessfulLogin(string email);
        bool LogFailedLoginAttempt(string emailAddress);
        ForgotPassword IsUsernameAvailable(Guid validator);
        bool ResetPassword(string hash, string salt, string email);
        Guid GetVisitorUid();
    }
}
