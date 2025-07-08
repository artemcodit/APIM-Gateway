using Capi.Management.Dtos;
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

            // 1. Create or Update the Service (this is idempotent)
            var servicePayload = new { name = api.Name, url = api.UpstreamUrl, tags = api.Tags };
            var serviceJson = new StringContent(JsonSerializer.Serialize(servicePayload), Encoding.UTF8, "application/json");
            var serviceResponse = await _client.PutAsync($"{kongAdminUrl}/services/{api.Name}", serviceJson);
            serviceResponse.EnsureSuccessStatusCode();

            // 2. Check if the Route exists
            var getRouteResponse = await _client.GetAsync($"{kongAdminUrl}/services/{api.Name}/routes/{api.Name}");

            var routePayload = new
            {
                name = api.Name,
                paths = new[] { api.Route },
                methods = api.Methods,
                hosts = api.Hosts,
                tags = api.Tags
            };
            var routeJson = new StringContent(JsonSerializer.Serialize(routePayload), Encoding.UTF8, "application/json");
            HttpResponseMessage routeResponse;

            if (getRouteResponse.IsSuccessStatusCode)
            {
                // 3a. If Route exists, update it with PATCH
                routeResponse = await _client.PatchAsync($"{kongAdminUrl}/routes/{api.Name}", routeJson);
            }
            else if (getRouteResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // 3b. If Route does not exist, create it with POST
                routeResponse = await _client.PostAsync($"{kongAdminUrl}/services/{api.Name}/routes", routeJson);
            }
            else
            {
                // Handle other potential errors from the GET request
                getRouteResponse.EnsureSuccessStatusCode();
                return; 
            }
            
            routeResponse.EnsureSuccessStatusCode();

            // 4. Enable or disable the API by adding/removing the request-termination plugin
            await HandleApiEnabling(api, kongAdminUrl);
        }

        private async Task HandleApiEnabling(Api api, string kongAdminUrl)
        {
            // First, find if the plugin is already attached to the route
            var pluginsResponse = await _client.GetAsync($"{kongAdminUrl}/routes/{api.Name}/plugins");
            if (!pluginsResponse.IsSuccessStatusCode) return;

            var pluginsStream = await pluginsResponse.Content.ReadAsStreamAsync();
            var plugins = await JsonSerializer.DeserializeAsync<JsonElement>(pluginsStream);

            var requestTerminationPlugin = plugins.GetProperty("data").EnumerateArray()
                .FirstOrDefault(p => p.GetProperty("name").GetString() == "request-termination");

            var pluginId = requestTerminationPlugin.ValueKind != JsonValueKind.Undefined ? requestTerminationPlugin.GetProperty("id").GetString() : null;

            if (api.IsEnabled && pluginId != null)
            {
                // API is enabled, so delete the plugin if it exists
                await _client.DeleteAsync($"{kongAdminUrl}/routes/{api.Name}/plugins/{pluginId}");
            }
            else if (!api.IsEnabled && pluginId == null)
            {
                // API is disabled, so add the plugin if it doesn't exist
                var pluginPayload = new
                {
                    name = "request-termination",
                    config = new { status_code = 503, message = "API is disabled" }
                };
                var pluginJson = new StringContent(JsonSerializer.Serialize(pluginPayload), Encoding.UTF8, "application/json");
                await _client.PostAsync($"{kongAdminUrl}/routes/{api.Name}/plugins", pluginJson);
            }
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
