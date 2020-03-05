using Microsoft.Extensions.Options;
using StoreModel.Account;
using StoreModel.Generic;
using StoreRepository.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StoreRepository
{
    public class UsersRepository : BaseRepository, IUsersRepository
    {

        public UsersRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public Users GetUserByUid(Guid userUid)
        {
            const string sql = "SELECT * FROM SiteUser WHERE [Uid] = @UserUid AND SiteId = @SiteId;";

            var user = Query<Users>(sql, new { UserUid = userUid, SiteId = siteId }).FirstOrDefault();

            return user;
        }
        public Users GetUserByEmail(string email)
        {
            const string sql = "SELECT * FROM SiteUser WHERE [EmailAddress] = @EmailAddress AND SiteId = @SiteId;";

            var user = Query<Users>(sql, new { EmailAddress = email, SiteId = siteId }).FirstOrDefault();

            return user;
        }

        public Users Register(Users user)
        {

            const string sql = @"
IF(NOT EXISTS( SELECT * FROM SiteUser WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId))
BEGIN
    INSERT INTO SiteUser
    SELECT NEWID(), @SiteId, @Username, @EmailAddress, @Hash, @Salt, @FirstName, @LastName, @Active, 0, GETDATE(), 0, NEWID(), GETDATE(), GETDATE()

    SELECT * FROM SiteUser WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId;
END
";
            var newUser = Query<Users>(sql,
                new
                {
                    SiteId = siteId,
                    Username = user.Username,
                    EmailAddress = user.EmailAddress,
                    Hash = user.Hash,
                    Salt = user.Salt,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Active = user.Active
                }).FirstOrDefault();

            return newUser;
        }

        public bool UsernameExists(string email)
        {
            const string sql = @"
SELECT CAST(COUNT(Id) AS BIT) FROM SiteUser WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId 
";
            return Query<bool>(sql, new { EmailAddress = email, SiteId = siteId }).FirstOrDefault();
        }

        public Guid? GeneratePasswordResetValidator(string email)
        {
            var validator = Query<Guid?>(@"
IF EXISTS(SELECT TOP 1 * FROM SiteUser WHERE EmailAddress = @EmailAddress and SiteId = @SiteId)
BEGIN 
	UPDATE SiteUser SET Validator = NEWID(), LastModifiedDate = GETDATE() 
	WHERE EmailAddress = @EmailAddress and SiteId = @SiteId;

    SELECT TOP 1 Validator FROM SiteUser 
	WHERE EmailAddress = @EmailAddress and SiteId = @SiteId;
END
ELSE
BEGIN
	SELECT NULL;
END", new { EmailAddress = email, SiteId = siteId }).FirstOrDefault();

            return validator;
        }


        public Users ValidateEmail(Guid guid)
        {
            Users user = null;
            const string sql = @"
DECLARE @Email NVARCHAR(100);

SET @Email = (SELECT TOP 1 EmailAddress FROM SiteUser WHERE Validator = @Validator AND SiteId = @SiteId);

IF EXISTS(SELECT * FROM SiteUser WHERE Validator = @Validator AND SiteId = @SiteId)
BEGIN
	UPDATE SiteUser SET Active = 0, Validator = NULL WHERE Validator = @Validator AND SiteId = @SiteId

	SELECT TOP 1 * FROM SiteUser WHERE EmailAddress = @Email AND SiteId = @SiteId;
END;
";
            user = Query<Users>(sql, new { Validator = guid, SiteId = siteId }).FirstOrDefault();
            return user;
        }

        public AuthLoginAttempt GetParticipantByUsername(string email)
        {
            const string sql = @"
SELECT Id, [Uid],  [Hash], Salt, EmailAddress FROM SiteUser WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId;
";
            var authLoginAttempt = Query<AuthLoginAttempt>(sql, new { EmailAddress = email, SiteId = siteId }).SingleOrDefault();
            return authLoginAttempt;
        }

        public bool LogSuccessfulLogin(string email)
        {
            const string sql = @"
DECLARE @Id INT;
SELECT @Id = Id from SiteUser where EmailAddress = @EmailAddress AND SiteId = @SiteId;
IF @Id IS NOT NULL
BEGIN
    UPDATE SiteUser SET LoginAttempts = 0, LastLoginDate = GETDATE(), LastModifiedDate = GETDATE() WHERE Id = @Id;
    
	SELECT 1;
END
ELSE
BEGIN
	SELECT 0;
END
";
            var success = Query<bool>(sql, new { EmailAddress = email, SiteId = siteId }).SingleOrDefault();
            return success;
        }

        public bool LogFailedLoginAttempt(string emailAddress)
        {
            const string sql = @"
IF EXISTS(SELECT ISNULL(LoginAttempts, 0) FROM SiteUser WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId)
BEGIN
    UPDATE SiteUser SET LoginAttempts = LoginAttempts+1, LastModifiedDate = GETDATE() WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId;
	SELECT 1
END
ELSE
BEGIN
	SELECT 0
END
";
            var failed = Query<bool>(sql, new { EmailAddress = emailAddress, SiteId = siteId }).SingleOrDefault();
            return failed;
        }
        public ForgotPassword IsUsernameAvailable(Guid validator)
        {
            const string sql = @"
IF(EXISTS(SELECT * FROM Registrant r JOIN UserDetails u ON r.Id = u.RegistrantId WHERE u.Validator = @Validator))
BEGIN
    SELECT TOP 1 r.Id, r.Uid, r.EmailAddress as Email, 1 AS Status, u.Id as UserId, u.Uid as UserGuid, r.FirstName, r.LastNameFROM Registrant r 
    JOIN UserDetails u ON r.Id = u.RegistrantId
    WHERE u.Validator = @Validator AND u.SiteId = @SiteId;
END
ELSE
    SELECT 0 AS Status;
";

            var isUsernameAvailable = Query<ForgotPassword>(sql, new { Validator = validator, SiteId = siteId }).FirstOrDefault();
            return isUsernameAvailable;
        }

        public bool ResetPassword(string hash, string salt, string email)
        {
            const string sql = @"
IF(EXISTS(SELECT * FROM SiteUser  WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId))
BEGIN
	UPDATE SiteUser SET Salt = @Salt, [Hash] = @Hash, LastModifiedDate = GETDATE() 
	WHERE EmailAddress = @EmailAddress AND SiteId = @SiteId;
	
	SELECT 1;
END 
ELSE 
	SELECT 0;
";

            return Query<bool>(sql, new { Hash = hash, Salt = salt, SiteId = siteId, EmailAddress = email }).FirstOrDefault();
        }

        public async Task<Guid> GetVisitorUid()
        {
            const string sql = @"
DECLARE @InsertItems TABLE ([Uid] UNIQUEIDENTIFIER)
	INSERT INTO Visitor
	OUTPUT INSERTED.VisitorUid INTO @InsertItems
	SELECT NEWID(), NULL, GETDATE(), GETDATE(),NULL;
	
	SELECT TOP 1 [Uid] FROM @InsertItems
";
            var guid = await QueryAsync<Guid>(sql).ConfigureAwait(false);
            return guid.FirstOrDefault();
        }
    }
}
