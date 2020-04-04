using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using StoreService.Interface;

namespace ShirtStoreService.Controllers.Filters
{
    public class VisitorMiddleware
    {
        private readonly RequestDelegate _next;

        public VisitorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAccountService _accountService)
        {
            string uidHeader = context.Request.Headers["visitor"].ToString();

            Guid uid;
            bool isValidUidParse = Guid.TryParse(uidHeader, out uid);

            if (string.IsNullOrEmpty(uidHeader) || !isValidUidParse || uid == Guid.Empty)
                uid = await _accountService.GetVisitorUid().ConfigureAwait(false);

            context.Response.OnStarting((Func<Task>)(() =>
            {
                context.Response.Headers.Add("X-Custom-VM", uid.ToString());
                context.Response.Headers.Add("visitor-VM", uid.ToString());
                context.Response.Headers.Add("Access-Control-Expose-Headers", "X-Custom-VM, visitor-VM");
                return Task.CompletedTask;
            }));

            await this._next(context);
        }
    }
}
