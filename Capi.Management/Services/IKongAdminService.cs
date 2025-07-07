using Capi.Management.Models;

namespace Capi.Management.Services
{
 /// Defines the contract for a service that interacts with the Kong Admin API.
    public interface IKongAdminService
    {
         /// Creates or updates a service and its corresponding route in Kong.
        /// <param name="api">The API to create or update in Kong.</param>
        Task CreateOrUpdateServiceAndRoute(Api api);

         /// Deletes a service and its corresponding route from Kong.
        /// <param name="apiName">The name of the API to delete.</param>
        Task DeleteServiceAndRoute(string apiName);
    }
}
