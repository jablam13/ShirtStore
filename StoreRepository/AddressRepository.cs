using Microsoft.Extensions.Options;
using StoreModel.Account;
using StoreModel.Generic;
using StoreRepository.Interface;
using System;
using System.Linq;

namespace StoreRepository
{
    public class AddressRepository : BaseRepository, IAddressRepository
    {
        public AddressRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        { }

        public UserAddress CreateAddress(UserAddress address)
        {
            const string sql = @"
DECLARE @InsertedAddress TABLE ([Id] INT);

--Insert new address
IF EXISTS(SELECT * FROM [Address] WHERE 1=1 AND
	FirstName = @FirstName AND
	LastName = @LastName AND 
	Street = @Street AND 
	Street1 = @Street1 AND
	City = @City AND 
	StateCode = @StateCode AND
	ZipCode = @ZipCode)
BEGIN
	IF EXISTS(SELECT TOP 1 * FROM [Address] WHERE 1=1 AND
	FirstName = @FirstName AND
	LastName = @LastName AND 
	Street = @Street AND 
	Street1 = @Street1 AND
	City = @City AND 
	StateCode = @StateCode AND
	ZipCode = @ZipCode AND
	Active = 0)
    BEGIN 
        UPDATE [Adress] SET Active = 1 WHERE FirstName = @FirstName AND
			LastName = @LastName AND 
			Street = @Street AND 
			Street1 = @Street1 AND
			City = @City AND 
			StateCode = @StateCode AND
			ZipCode = @ZipCode;
    END

	SELECT TOP 1 * FROM [Address] WHERE 1=1 AND
		FirstName = @FirstName AND
		LastName = @LastName AND 
		Street = @Street AND 
		Street1 = @Street1 AND
		City = @City AND 
		StateCode = @StateCode AND
		ZipCode = @ZipCode;
END
ELSE
	INSERT INTO [Address]
	OUTPUT INSERTED.Id INTO @InsertedAddress
	SELECT NEWID(), @UserId, @Token, @FirstName, @LastName, @Street, @Street1, @City, 
    @StateCode, @ZipCode, @IsBilling, 1, @PrimaryAddress, GETDATE(), GETDATE();

SELECT TOP 1 * FROM [Address] WHERE Id = (SELECT TOP 1 Id FROM @InsertedAddress);";

            var a = Query<UserAddress>(sql, new
            {
                address.UserId,
                address.Token,
                FirstName = address.FirstName.Trim(),
                LastName = address.LastName.Trim(),
                Street = address.Street?.Trim(),
                Street1 = address.Street1?.Trim(),
                City = address.City?.Trim(),
                StateCode = address.StateCode?.Trim(),
                ZipCode = address.ZipCode.Trim(),
                address.IsBilling,
                address.PrimaryAddress,
            }).FirstOrDefault();

            return a;
        }

        public UserAddress EditAddress(UserAddress address)
        {
            return new UserAddress();
        }

        public void RemoveAddress(UserAddress address)
        { }

        public UserAddress GetAddress(UserAddress address)
        {
            const string sql = @"
	SELECT TOP 1 * FROM [Address] WHERE 1=1 AND
		FirstName = @FirstName AND
		LastName = @LastName AND 
		(Street = @Street OR @Street IS NULL) AND 
		(Street1 = @Street1 OR @Street1 IS NULL) AND
		(City = @City OR @City IS NULL) AND 
		(StateCode = @StateCode OR @StateCode IS NULL) AND
		ZipCode = @ZipCode AND 
        IsBilling = @IsBilling;";

            var a = Query<UserAddress>(sql, new
            {
                address.UserId,
                address?.Token,
                FirstName = address.FirstName.Trim(),
                LastName = address.LastName.Trim(),
                Street = address.Street?.Trim(),
                Street1 = address.Street1?.Trim(),
                City = address.City?.Trim(),
                StateCode = address.StateCode?.Trim(),
                ZipCode = address.ZipCode.Trim(),
                address.IsBilling,
                address.PrimaryAddress,
            }).FirstOrDefault();

            return a;
        }

        public UserAddress GetAddress(int addressId)
        {
            return new UserAddress();
        }

        public UserAddress GetAddress(Guid addressUid)
        {
            return new UserAddress();
        }
    }
}
