using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capi.Management.Dtos;
using Capi.Management.Models;

namespace Capi.Management.Services
{
    public interface IApiProductService
    {
        Task<IEnumerable<ApiProductDto>> GetAllApiProductsAsync();
        Task<ApiProductDto?> GetApiProductByIdAsync(Guid id);
        Task<ApiProductDto> CreateApiProductAsync(ApiProductCreateDto createDto);
        Task<bool> UpdateApiProductAsync(Guid id, ApiProductUpdateDto updateDto);
        Task<bool> DeleteApiProductAsync(Guid id);
        Task<bool> AddApiToProductAsync(Guid productId, Guid apiId);
        Task<bool> RemoveApiFromProductAsync(Guid productId, Guid apiId);
        Task<IEnumerable<ApiDto>> GetApisForProductAsync(Guid productId);
    }
}
