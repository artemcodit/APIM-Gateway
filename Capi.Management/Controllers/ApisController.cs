using Capi.Management.Dtos;
using Capi.Management.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Capi.Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApisController : ControllerBase
    {
        private readonly IApiService _apiService;

        public ApisController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetApis()
        {
            var apis = await _apiService.GetApisAsync();
            var apiDtos = apis.Select(api => new ApiDto
            {
                Id = api.Id,
                Name = api.Name,
                Route = api.Route,
                UpstreamUrl = api.UpstreamUrl,
                Methods = api.Methods,
                IsEnabled = api.IsEnabled,
                ApiProducts = api.ApiProducts.Select(p => new ApiProductDto { Id = p.Id, Name = p.Name, Description = p.Description }).ToList()
            });
            return Ok(apiDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApi(Guid id)
        {
            var api = await _apiService.GetApiByIdAsync(id);
            if (api == null)
            {
                return NotFound();
            }
            var apiDto = new ApiDto
            {
                Id = api.Id,
                Name = api.Name,
                Route = api.Route,
                UpstreamUrl = api.UpstreamUrl,
                Methods = api.Methods,
                IsEnabled = api.IsEnabled,
                ApiProducts = api.ApiProducts.Select(p => new ApiProductDto { Id = p.Id, Name = p.Name, Description = p.Description }).ToList()
            };
            return Ok(apiDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateApi([FromBody] ApiCreateDto apiCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newApi = await _apiService.CreateApiAsync(apiCreateDto);
                var newApiDto = new ApiDto
                {
                    Id = newApi.Id,
                    Name = newApi.Name,
                    Route = newApi.Route,
                    UpstreamUrl = newApi.UpstreamUrl,
                    Methods = newApi.Methods,
                    IsEnabled = newApi.IsEnabled,
                    ApiProducts = newApi.ApiProducts.Select(p => new ApiProductDto { Id = p.Id, Name = p.Name, Description = p.Description }).ToList()
                };
                return CreatedAtAction(nameof(GetApi), new { id = newApiDto.Id }, newApiDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApi(Guid id, [FromBody] ApiUpdateDto apiUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _apiService.UpdateApiAsync(id, apiUpdateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApi(Guid id)
        {
            await _apiService.DeleteApiAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/test")]
        public async Task<IActionResult> TestApi(Guid id, [FromBody] ApiTestRequestDto testRequest)
        {
            try
            {
                var result = await _apiService.TestApiAsync(id, testRequest);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Bad Gateway", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", details = ex.Message });
            }
        }
    }
}
