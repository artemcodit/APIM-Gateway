using Capi.Management.Models;
using System.Text;
using System.Text.Json;

namespace Capi.Management.Services
{
    
    /// Service for interacting with the Kong Admin API to manage services and routes.
    
    public class KongAdminService : IKongAdminService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        
        /// Initializes a new instance of the <see cref="KongAdminService"/> class.
        
        /// <param name="client">The HTTP client.</param>
        /// <param name="configuration">The configuration.</param>
        public KongAdminService(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        
        /// Creates or updates a service and its corresponding route in Kong.
        
        /// <param name="api">The API to create or update in Kong.</param>
        public async Task CreateOrUpdateServiceAndRoute(Api api)
        {
            var kongAdminUrl = _configuration["Kong:AdminUrl"];

            // Create or update service in Kong
            var service = new { name = api.Name, url = api.UpstreamUrl };
            var serviceJson = new StringContent(JsonSerializer.Serialize(service), Encoding.UTF8, "application/json");
            // Use PUT to create or update the service idempotently
            await _client.PutAsync($"{kongAdminUrl}/services/{api.Name}", serviceJson);

            // Create or update route in Kong
            var route = new { name = api.Name, paths = new[] { api.Route }, methods = api.Methods };
            var routeJson = new StringContent(JsonSerializer.Serialize(route), Encoding.UTF8, "application/json");
            // Use PUT to create or update the route idempotently
            await _client.PutAsync($"{kongAdminUrl}/services/{api.Name}/routes/{api.Name}", routeJson);
        }

        
        /// Deletes a service and its corresponding route from Kong.
        
        /// <param name="apiName">The name of the API to delete.</param>
        public async Task DeleteServiceAndRoute(string apiName)
        {
            var kongAdminUrl = _configuration["Kong:AdminUrl"];

            // Delete the route first
            await _client.DeleteAsync($"{kongAdminUrl}/services/{apiName}/routes/{apiName}");
            // Then delete the service
            await _client.DeleteAsync($"{kongAdminUrl}/services/{apiName}");
        }
    }
}
