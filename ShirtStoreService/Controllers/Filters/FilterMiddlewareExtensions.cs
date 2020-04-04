using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace ShirtStoreService.Controllers.Filters
{
    public static class FilterMiddlewareExtensions
    {
        public static IApplicationBuilder UseVisitorHeader(this IApplicationBuilder app)
        {
            return app.UseMiddleware<VisitorMiddleware>();
        }
    }
}
