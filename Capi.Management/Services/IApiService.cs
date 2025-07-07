using Capi.Management.Dtos;
using Capi.Management.Models;

namespace Capi.Management.Services
{
 /// Defines the contract for a service that manages APIs.
    public interface IApiService
    {
         /// Creates a new API.
        /// <param name="apiCreateDto">The data transfer object containing the API information.</param>
        /// <returns>The created API.</returns>
        Task<Api> CreateApiAsync(ApiCreateDto apiCreateDto);

         /// Retrieves all APIs.
        /// <returns>A collection of all APIs.</returns>
        Task<IEnumerable<Api>> GetApisAsync();

         /// Updates the policies for a specific API.
        /// <param name="id">The ID of the API to update.</param>
        /// <param name="policyDtos">The data transfer objects containing the new policies.</param>
        Task UpdateApiPoliciesAsync(Guid id, IEnumerable<PolicyDto> policyDtos);

         /// Deletes an API.
        /// <param name="id">The ID of the API to delete.</param>
        Task DeleteApiAsync(Guid id);
    }
}
