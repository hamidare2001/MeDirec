using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeDirect.ExchangeApi.Middlewares
{
    public static class CacheMiddleware
    {
        public static IApplicationBuilder UseCaching(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserCacheMiddleware>();
        }
    }

    public class UserCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserCacheMiddleware> _logger;

        public UserCacheMiddleware(RequestDelegate next, ILogger<UserCacheMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task Invoke(HttpContext context)
        {
            await FormatRequest(context.Request);
            await _next(context);
        }

        private async Task  FormatRequest(HttpRequest request)
        { 
             
            request.EnableBuffering(); 
            var buffer = new byte[Convert.ToInt32(request.ContentLength)]; 
            var bodyAsText = await ReadBody(request, buffer);  

            request.Body.Position = 0;

            _logger.LogInformation("Processed {@Path} with input: {@Input}",$"{request.Host}{request.Path}", bodyAsText); 
           
        }

        private async Task<string> ReadBody(HttpRequest request, byte[] buffer)
        {
            try
            {
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                var bodyAsText = Encoding.UTF8.GetString(buffer);

                return bodyAsText;
            }
            catch (Exception e)
            {
                _logger.LogError("Error in read body: " + request.Path);
                return string.Empty;
            }
        }
    }
}
