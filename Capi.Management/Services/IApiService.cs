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

        /// <summary>
        /// Retrieves a specific API by its ID.
        /// </summary>
        /// <param name="id">The ID of the API to retrieve.</param>
        /// <returns>The requested API.</returns>
        Task<Api> GetApiByIdAsync(Guid id);

        /// <summary>
        /// Updates an existing API.
        /// </summary>
        /// <param name="id">The ID of the API to update.</param>
        /// <param name="apiUpdateDto">The data transfer object containing the updated API information.</param>
        /// <returns>The updated API.</returns>
        Task<Api> UpdateApiAsync(Guid id, ApiUpdateDto apiUpdateDto);

         /// Updates the policies for a specific API.
        /// <param name="id">The ID of the API to update.</param>
        /// <param name="policyDtos">The data transfer objects containing the new policies.</param>
        Task UpdateApiPoliciesAsync(Guid id, IEnumerable<PolicyDto> policyDtos);

         /// Deletes an API.
        /// <param name="id">The ID of the API to delete.</param>
        Task DeleteApiAsync(Guid id);

        /// <summary>
        /// Tests an API by sending a request to its upstream URL.
        /// </summary>
        /// <param name="id">The ID of the API to test.</param>
        /// <param name="testRequest">The request details for the test.</param>
        /// <returns>The response from the API test.</returns>
        Task<ApiTestResponseDto> TestApiAsync(Guid id, ApiTestRequestDto testRequest);
    }
}
