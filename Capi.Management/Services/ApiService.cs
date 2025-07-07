using Capi.Management.Data;
using Capi.Management.Dtos;
using Capi.Management.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
            var api = new Api
            {
                Name = apiCreateDto.Name,
                Route = apiCreateDto.Route,
                UpstreamUrl = apiCreateDto.UpstreamUrl,
                Methods = apiCreateDto.Methods
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
                return JsonSerializer.Deserialize<IEnumerable<Api>>(cachedApis);
            }

            // If not in cache, get from database
            var apis = await _context.Apis.Include(a => a.Policies).ToListAsync();
            
            // Serialize and store in cache for future requests
            await _cache.SetStringAsync(ApisCacheKey, JsonSerializer.Serialize(apis), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache for 10 minutes
            });

            return apis;
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
                // First, delete the service and route from Kong
                await _kongAdminService.DeleteServiceAndRoute(api.Name);

                // Then, remove from the database
                _context.Apis.Remove(api);
                await _context.SaveChangesAsync();
                // Invalidate the cache
                await _cache.RemoveAsync(ApisCacheKey);
            }
        }
    }
}
