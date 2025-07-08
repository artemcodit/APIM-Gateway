using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capi.Management.Dtos;
using Capi.Management.Services;
using Microsoft.AspNetCore.Mvc;

namespace Capi.Management.Controllers
{
    [ApiController]
    [Route("api/api-products")]
    public class ApiProductsController : ControllerBase
    {
        private readonly IApiProductService _apiProductService;

        public ApiProductsController(IApiProductService apiProductService)
        {
            _apiProductService = apiProductService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiProductDto>>> GetAllApiProducts()
        {
            var apiProducts = await _apiProductService.GetAllApiProductsAsync();
            return Ok(apiProducts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiProductDto>> GetApiProductById(Guid id)
        {
            var apiProduct = await _apiProductService.GetApiProductByIdAsync(id);
            if (apiProduct == null) return NotFound();
            return Ok(apiProduct);
        }

        [HttpPost]
        public async Task<ActionResult<ApiProductDto>> CreateApiProduct([FromBody] ApiProductCreateDto createDto)
        {
            var newApiProduct = await _apiProductService.CreateApiProductAsync(createDto);
            return CreatedAtAction(nameof(GetApiProductById), new { id = newApiProduct.Id }, newApiProduct);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateApiProduct(Guid id, [FromBody] ApiProductUpdateDto updateDto)
        {
            var result = await _apiProductService.UpdateApiProductAsync(id, updateDto);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApiProduct(Guid id)
        {
            var result = await _apiProductService.DeleteApiProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{productId}/apis")]
        public async Task<IActionResult> AddApiToProduct(Guid productId, [FromBody] Guid apiId)
        {
            var result = await _apiProductService.AddApiToProductAsync(productId, apiId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{productId}/apis/{apiId}")]
        public async Task<IActionResult> RemoveApiFromProduct(Guid productId, Guid apiId)
        {
            var result = await _apiProductService.RemoveApiFromProductAsync(productId, apiId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpGet("{productId}/apis")]
        public async Task<ActionResult<IEnumerable<ApiDto>>> GetApisForProduct(Guid productId)
        { 
            var apis = await _apiProductService.GetApisForProductAsync(productId);
            return Ok(apis);
        }
    }
}
