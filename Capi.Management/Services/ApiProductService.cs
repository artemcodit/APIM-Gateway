using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capi.Management.Data;
using Capi.Management.Dtos;
using Capi.Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Capi.Management.Services
{
    public class ApiProductService : IApiProductService
    {
        private readonly ApiDbContext _context;

        public ApiProductService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<ApiProductDto> CreateApiProductAsync(ApiProductCreateDto createDto)
        {
            var apiProduct = new ApiProduct
            {
                Name = createDto.Name,
                Description = createDto.Description,
            };

            _context.ApiProducts.Add(apiProduct);
            await _context.SaveChangesAsync();

            return new ApiProductDto
            {
                Id = apiProduct.Id,
                Name = apiProduct.Name,
                Description = apiProduct.Description,
                CreatedAt = apiProduct.CreatedAt,
                UpdatedAt = apiProduct.UpdatedAt,
                ApiCount = 0
            };
        }

        public async Task<bool> DeleteApiProductAsync(Guid id)
        {
            var apiProduct = await _context.ApiProducts.FindAsync(id);
            if (apiProduct == null) return false;

            _context.ApiProducts.Remove(apiProduct);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ApiProductDto>> GetAllApiProductsAsync()
        {
            return await _context.ApiProducts
                .Select(p => new ApiProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    ApiCount = p.Apis.Count
                })
                .ToListAsync();
        }

        public async Task<ApiProductDto?> GetApiProductByIdAsync(Guid id)
        {
            var apiProduct = await _context.ApiProducts
                .Include(p => p.Apis)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (apiProduct == null) return null;

            return new ApiProductDto
            {
                Id = apiProduct.Id,
                Name = apiProduct.Name,
                Description = apiProduct.Description,
                CreatedAt = apiProduct.CreatedAt,
                UpdatedAt = apiProduct.UpdatedAt,
                ApiCount = apiProduct.Apis.Count
            };
        }

        public async Task<bool> UpdateApiProductAsync(Guid id, ApiProductUpdateDto updateDto)
        {
            var apiProduct = await _context.ApiProducts.FindAsync(id);
            if (apiProduct == null) return false;

            if (!string.IsNullOrEmpty(updateDto.Name))
            {
                apiProduct.Name = updateDto.Name;
            }

            if (updateDto.Description != null)
            {
                apiProduct.Description = updateDto.Description;
            }

            apiProduct.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddApiToProductAsync(Guid productId, Guid apiId)
        {
            var apiProduct = await _context.ApiProducts.Include(p => p.Apis).FirstOrDefaultAsync(p => p.Id == productId);
            var api = await _context.Apis.FindAsync(apiId);

            if (apiProduct == null || api == null)
            {
                return false;
            }

            if (!apiProduct.Apis.Any(a => a.Id == apiId))
            {
                apiProduct.Apis.Add(api);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveApiFromProductAsync(Guid productId, Guid apiId)
        {
            var apiProduct = await _context.ApiProducts.Include(p => p.Apis).FirstOrDefaultAsync(p => p.Id == productId);
            var api = await _context.Apis.FindAsync(apiId);

            if (apiProduct == null || api == null)
            {
                return false;
            }

            var apiToRemove = apiProduct.Apis.FirstOrDefault(a => a.Id == apiId);
            if (apiToRemove != null)
            {
                apiProduct.Apis.Remove(apiToRemove);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<IEnumerable<ApiDto>> GetApisForProductAsync(Guid productId)
        {
            var apiProduct = await _context.ApiProducts
                .Include(p => p.Apis)
                .ThenInclude(a => a.Policies)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (apiProduct == null) return new List<ApiDto>();

            return apiProduct.Apis.Select(a => new ApiDto
            {
                Id = a.Id,
                Name = a.Name,
                Route = a.Route,
                UpstreamUrl = a.UpstreamUrl,
                Methods = a.Methods,
                Hosts = a.Hosts,
                Tags = a.Tags,
                IsEnabled = a.IsEnabled,
                Policies = a.Policies.Select(p => new PolicyDto { Type = p.Type, Configuration = p.Configuration }).ToList()
            }).ToList();
        }
    }
}
