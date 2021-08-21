using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Middleware
{
    public class RequestConsoleLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestConsoleLoggingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} -> {context.Request.Host} -> {context.Request.Method} {context.Request.Path} -> ";
            await _next(context);
            Console.WriteLine(string.Concat(message, context.Response.StatusCode.ToString()));
        }
    }
}
