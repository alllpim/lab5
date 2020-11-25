using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kindergarten.Data;

namespace Kindergarten.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class InitializeRolesMiddleware
    {
        private readonly RequestDelegate _next;

        public InitializeRolesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (!(httpContext.Session.Keys.Contains("starting")))
            {
                InitializeRoles.Initialize(httpContext).Wait();
                httpContext.Session.SetString("starting", "Yes");
            }
            return _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class InitializeRolesMiddlewareExtensions
    {
        public static IApplicationBuilder UseInitializeRolesMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InitializeRolesMiddleware>();
        }
    }
}
