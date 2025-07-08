using Capi.Management.Data;
using Capi.Management.Dtos;
using Capi.Management.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Capi.Management.Services
{
    /// Service for managing APIs, including CRUD operations and synchronization with Kong.
    public class ApiService : IApiService
    {
        private readonly ApiDbContext _context;
        private readonly IKongAdminService _kongAdminService;
        private readonly IDistributedCache _cache;
        private const string ApisCacheKey = "apis";

         /// Initializes a new instance of the <see cref="ApiService"/> class.
        /// <param name="context">The database context.</param>
        /// <param name="kongAdminService">The Kong admin service.</param>
        /// <param name="cache">The distributed cache.</param>
        public ApiService(ApiDbContext context, IKongAdminService kongAdminService, IDistributedCache cache)
        {
            _context = context;
            _kongAdminService = kongAdminService;
            _cache = cache;
        }

         /// Creates a new API, saves it to the database, syncs with Kong, and invalidates the cache.
        /// <param name="apiCreateDto">The data transfer object containing the API information.</param>
        /// <returns>The created API.</returns>
        public async Task<Api> CreateApiAsync(ApiCreateDto apiCreateDto)
        {
            var apiProduct = await _context.ApiProducts.FindAsync(apiCreateDto.ApiProductId);
            if (apiProduct == null)
            {
                throw new ArgumentException("Invalid ApiProductId");
            }

            var api = new Api
            {
                Name = apiCreateDto.Name,
                Route = apiCreateDto.Route,
                UpstreamUrl = apiCreateDto.UpstreamUrl,
                Methods = apiCreateDto.Methods ?? new List<string>(),
                Hosts = apiCreateDto.Hosts ?? new List<string>(),
                Tags = apiCreateDto.Tags ?? new List<string>(),
                IsEnabled = apiCreateDto.IsEnabled,
                ApiProducts = new List<ApiProduct> { apiProduct }
            };

            _context.Apis.Add(api);
            await _context.SaveChangesAsync();

            // After saving to DB, create/update the service and route in Kong
            await _kongAdminService.CreateOrUpdateServiceAndRoute(api);
            // Invalidate the cache
            await _cache.RemoveAsync(ApisCacheKey);

            return api;
        }

         /// Retrieves all APIs, using a cache-aside pattern.
        /// <returns>A collection of all APIs.</returns>
        public async Task<IEnumerable<Api>> GetApisAsync()
        {
            // Try to get APIs from cache
            var cachedApis = await _cache.GetStringAsync(ApisCacheKey);
            if (!string.IsNullOrEmpty(cachedApis))
            {
                // If found, deserialize and return
                return JsonSerializer.Deserialize<IEnumerable<Api>>(cachedApis, new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve });
            }

            // If not in cache, get from database
            var apis = await _context.Apis
                .Include(a => a.Policies)
                .Include(a => a.ApiProducts)
                .ToListAsync();
            
            // Serialize and store in cache for future requests
            await _cache.SetStringAsync(ApisCacheKey, JsonSerializer.Serialize(apis, new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve }), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
            });

            return apis;
        }

        /// <inheritdoc />
        public async Task<Api> GetApiByIdAsync(Guid id)
        {
            // First, try to get the API from the cache
            var cachedApi = await _cache.GetStringAsync($"api_{id}");
            if (!string.IsNullOrEmpty(cachedApi))
            {
                return JsonSerializer.Deserialize<Api>(cachedApi, new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve });
            }

            // If not in cache, get from the database
            var api = await _context.Apis
                .Include(a => a.Policies)
                .Include(a => a.ApiProducts)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (api != null)
            {
                await _cache.SetStringAsync($"api_{id}", JsonSerializer.Serialize(api, new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve }), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return api;
        }

        /// <inheritdoc />
        public async Task<Api> UpdateApiAsync(Guid id, ApiUpdateDto apiUpdateDto)
        {
            var api = await _context.Apis.Include(a => a.ApiProducts).FirstOrDefaultAsync(a => a.Id == id);

            if (api == null)
            {
                return null; // Or throw an exception
            }

            api.Name = apiUpdateDto.Name;
            api.Route = apiUpdateDto.Route;
            api.UpstreamUrl = apiUpdateDto.UpstreamUrl;
            api.Methods = apiUpdateDto.Methods;
            api.Hosts = apiUpdateDto.Hosts;
            api.Tags = apiUpdateDto.Tags;
            api.IsEnabled = apiUpdateDto.IsEnabled;

            if (apiUpdateDto.ApiProductId.HasValue && (api.ApiProducts.FirstOrDefault()?.Id != apiUpdateDto.ApiProductId.Value))
            {
                var product = await _context.ApiProducts.FindAsync(apiUpdateDto.ApiProductId.Value);
                if (product != null)
                {
                    api.ApiProducts.Clear();
                    api.ApiProducts.Add(product);
                }
            }

            await _context.SaveChangesAsync();

            // Update Kong
            await _kongAdminService.CreateOrUpdateServiceAndRoute(api);

            // Invalidate caches
            await _cache.RemoveAsync(ApisCacheKey); // Invalidate the list cache
            await _cache.RemoveAsync($"api_{id}"); // Invalidate the specific API cache

            return api;
        }

         /// Updates the policies for a specific API, saves changes to the database, syncs with Kong, and invalidates the cache.
        /// <param name="id">The ID of the API to update.</param>
        /// <param name="policyDtos">The data transfer objects containing the new policies.</param>
        public async Task UpdateApiPoliciesAsync(Guid id, IEnumerable<PolicyDto> policyDtos)
        {
            var api = await _context.Apis.Include(a => a.Policies).FirstOrDefaultAsync(a => a.Id == id);

            if (api != null)
            {
                // Replace existing policies with the new ones
                api.Policies = policyDtos.Select(p => new Policy { Type = p.Type, Configuration = p.Configuration }).ToList();
                await _context.SaveChangesAsync();

                // After updating policies, update the service and route in Kong
                await _kongAdminService.CreateOrUpdateServiceAndRoute(api);
                // Invalidate the cache
                await _cache.RemoveAsync(ApisCacheKey);
            }
        }

         /// Deletes an API from the database, removes it from Kong, and invalidates the cache.
        /// <param name="id">The ID of the API to delete.</param>
        public async Task DeleteApiAsync(Guid id)
        {
            var api = await _context.Apis.FindAsync(id);

            if (api != null)
            {
                // Remove from Kong first
                await _kongAdminService.DeleteServiceAndRoute(api.Name);

                // Then, remove from the database
                _context.Apis.Remove(api);
                await _context.SaveChangesAsync();
                // Invalidate the cache
                await _cache.RemoveAsync(ApisCacheKey);
                await _cache.RemoveAsync($"api_{id}");
            }
        }

        public async Task<ApiTestResponseDto> TestApiAsync(Guid id, ApiTestRequestDto testRequest)
        {
            var api = await _context.Apis.FindAsync(id);
            if (api == null)
            {
                throw new ArgumentException("API not found.");
            }

            using var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(testRequest.Method),
                RequestUri = new Uri(api.UpstreamUrl) // Assuming UpstreamUrl is the full URL
            };

            foreach (var header in testRequest.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(testRequest.Body))
            {
                // Default to application/json if content-type is not provided
                var contentType = testRequest.Headers.ContainsKey("Content-Type") 
                    ? testRequest.Headers["Content-Type"] 
                    : "application/json";
                request.Content = new StringContent(testRequest.Body, Encoding.UTF8, contentType);
            }

            var response = await client.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseHeaders = new Dictionary<string, IEnumerable<string>>();
            foreach (var header in response.Headers.Concat(response.Content.Headers))
            {
                responseHeaders[header.Key] = header.Value;
            }

            return new ApiTestResponseDto
            {
                StatusCode = (int)response.StatusCode,
                Headers = responseHeaders,
                Body = responseBody
            };
        }
    }
}
