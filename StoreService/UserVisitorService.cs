using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using StoreModel.Generic;
using StoreService.Interface;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace StoreService
{
    public class UserVisitorService : IUserVisitorService
    {
        private Guid visitorUid = Guid.Empty;
        private Guid userUid = Guid.Empty;
        private readonly AppSettings _appSettings;
        private readonly IAccountService accountService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserVisitorService(IOptions<AppSettings> appSettings, IHttpContextAccessor _httpContextAccessor, IAccountService _accountService)
        {
            accountService = _accountService;
            _appSettings = appSettings.Value;
            httpContextAccessor = _httpContextAccessor;
        }

        public async Task<Guid> GetVisitorUid()
        {
            string uidHeader = httpContextAccessor.HttpContext.Request.Headers["visitor"].ToString();

            Guid uid;
            bool isValidUidParse = Guid.TryParse(uidHeader, out uid);

            if (string.IsNullOrEmpty(uidHeader) || !isValidUidParse || uid == Guid.Empty)
                uid = await accountService.GetVisitorUid().ConfigureAwait(false);

            return uid;
        }


        public string GetIpValue()
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
