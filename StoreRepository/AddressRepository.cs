using Microsoft.Extensions.Options;
using StoreModel.Account;
using StoreModel.Generic;
using StoreRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreRepository
{
    public class AddressRepository : BaseRepository, IAddressRepository
    {
        public AddressRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        { }

        public async Task<UserAddress> CreateAddress(UserAddress address)
        {
            const string sql = @"
DECLARE @InsertedAddress TABLE ([Id] INT);
SELECT @UserId = Id FROM SiteUser WHERE Uid = @UserUid;

IF(@UserId IS NULL)
BEGIN 
    SELECT NULL;
END;


--Insert new address
IF NOT EXISTS(SELECT * FROM [Address] WHERE 1=1
	AND Street = @Street 
	AND Street2 = @Street2
	AND City = @City 
	AND StateCode = @StateCode
	AND ZipCode = @ZipCode
	AND Token = @Token
	AND GoogleId = @GoogleId)
BEGIN
	INSERT INTO [Address]
	OUTPUT INSERTED.Id INTO @InsertedAddress
	SELECT NEWID(), @Token, @GoogleId, @Street, @Street2, @City, @StateName, @StateCode, @ZipCode, 1, GETDATE(), GETDATE();
END
ELSE
BEGIN 
	INSERT INTO @InsertedAddress
	SELECT TOP 1 Id FROM [Address] WHERE 1=1
		AND Street = @Street 
		AND Street2 = @Street2
		AND City = @City 
		AND StateCode = @StateCode
		AND ZipCode = @ZipCode;
END;

IF NOT EXISTS(SELECT * FROM UserAddress WHERE 1=1
	AND AddressId = (SELECT Id From @InsertedAddress) 
	AND UserId = @UserId
	AND AddressTypeId = @AddressTypeId)
BEGIN 
	INSERT INTO UserAddress
	SELECT @UserId, Id, @AddressTypeId, @Token, @PrimaryAddress, 1, GETDATE(), GETDATE()  FROM @InsertedAddress;
END;

SELECT a.Id, a.Uid, ua.UserId, ua.AddressTypeId, ua.PrimaryAddress,
	a.Street, a.Street2, a.City, a.StateName, a.StateCode, a.ZipCode, a.GoogleId 
	FROM UserAddress ua
LEFT JOIN Address a ON a.Id = ua.AddressId
LEFT JOIN AddressType atp ON atp.Id = ua.AddressTypeId 
WHERE 1=1 
AND ua.Active = 1
AND a.Id = (SELECT Id FROM @InsertedAddress)
AND ua.UserId = @UserId;";

            var insertedAddress = await QueryAsync<UserAddress>(sql, address.TrimAddress()).ConfigureAwait(false);

            return insertedAddress.FirstOrDefault();
        }

        public async Task<UserAddress> EditAddress(UserAddress address)
        {

            const string sql = @"
UPDATE
    UserAddress
SET
    UserAddress.PrimaryAddress = @PrimaryAddress,
	UserAddress.AddressTypeId = @AddressTypeId,
	UserAddress.Active = @Active
FROM 
    UserAddress ua
    INNER JOIN [Address] a ON a.Id = ua.AddressId
	LEFT JOIN SiteUser su on su.Id = ua.UserId
WHERE 1=1
AND su.Uid = @UserUid
AND a.Id = @AddressId
AND su.SiteId = @SiteId;
";

            var updatedAddress = await QueryAsync<UserAddress>(sql, address.TrimAddress()).ConfigureAwait(false);

            return updatedAddress.FirstOrDefault();
        }

        public async Task<Guid?> RemoveAddress(Guid addressUid, Guid userUid)
        {
            const string sql = @"
DECLARE @RemovedAddressId TABLE ([Uid] UNIQUEIDENTIFIER);
DECLARE @UserId INT = (SELECT Id FROM SiteUser WHERE UserUid = @UserUid);

UPDATE [UserAddress] SET Active = 0 
OUTPUT INSERTED.[AddressId] INTO @RemovedAddressId
WHERE [Uid] = @Uid;

SELECT Uid FROM Address WHERE Id = @RemovedAddressId;
";
            var removedUid = await QueryAsync<Guid>(sql, new { Uid = addressUid }).ConfigureAwait(false);

            return removedUid.FirstOrDefault();
        }

        public async Task<List<UserAddress>> GetUserAddresses(UserAddress address)
        {
            const string sql = @"
SELECT ua.UserId, ua.AddressTypeId, ua.PrimaryAddress,
	a.Street, a.Street2, a.City, a.StateName, a.StateCode, a.ZipCode, a.GoogleId 
	FROM UserAddress ua
LEFT JOIN Address a ON a.Id = ua.AddressId
LEFT JOIN AddressType atp ON atp.Id = ua.AddressTypeId 
LEFT JOIN SiteUser su ON su.Id = ua.UserId
WHERE 1=1 
AND ua.Active = 1
AND a.Uid = @Uid
AND su.Uid = @UserUid;";

            var a = await QueryAsync<UserAddress>(sql, address.TrimAddress());

            return a.ToList();
        }

        public async Task<UserAddress> GetAddressById(int addressId)
        {
            const string sql = @"
SELECT ua.UserId, ua.AddressTypeId, ua.PrimaryAddress,
	a.Street, a.Street2, a.City, a.StateName, a.StateCode, a.ZipCode, a.GoogleId 
	FROM UserAddress ua
LEFT JOIN Address a ON a.Id = ua.AddressId
LEFT JOIN AddressType atp ON atp.Id = ua.AddressTypeId
WHERE 1=1 
AND ua.Active = 1
AND a.Id = @AddressId;
";
            var address = await QueryAsync<UserAddress>(sql, new { AddressId = addressId }).ConfigureAwait(false);

            return address.FirstOrDefault();
        }
    }

    public static class AddressExtensionMethod
    {
        public static UserAddress TrimAddress(this UserAddress address)
        {
            address.FirstName = address.FirstName?.Trim();
            address.LastName = address.LastName?.Trim();
            address.Street = address.Street?.Trim();
            address.Street2 = address.Street2?.Trim();
            address.City = address.City?.Trim();
            address.StateCode = address.StateCode?.Trim();
            address.StateName = address.StateName?.Trim();
            address.ZipCode = address.ZipCode?.Trim();

            return address;
        }
    }
}
