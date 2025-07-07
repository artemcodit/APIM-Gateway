using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Capi.Management.Middleware
{
    // This middleware is responsible for authenticating requests based on an API key.
    public class ApiKeyMiddleware
    {
        // The _next field holds a reference to the next middleware in the request pipeline.
        private readonly RequestDelegate _next;
        // This constant defines the name of the HTTP header that should contain the API key.
        private const string ApiKeyHeaderName = "X-Api-Key";

        // The constructor initializes the middleware with the next middleware in the pipeline.
        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // This method is invoked for each HTTP request.
        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            // Try to get the API key from the request headers.
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                // If the API key is not found, return a 401 Unauthorized response.
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key was not provided.");
                return;
            }

            // Get the configured API key from the application's configuration (appsettings.json).
            var apiKey = configuration.GetValue<string>("ApiKey");
            // Check if the configured API key is null or if it doesn't match the extracted API key.
            if (apiKey == null || !apiKey.Equals(extractedApiKey))
            {
                // If the API key is invalid, return a 401 Unauthorized response.
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            // If the API key is valid, call the next middleware in the pipeline.
            await _next(context);
        }
    }
}
