using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Dtos
{
    /// Data transfer object for creating a new API.
    public class ApiCreateDto
    {
        /// The name of the API.
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// The route (path) for the API.
        [Required]
        public string Route { get; set; } = string.Empty;

        /// The upstream URL for the API.
        [Required]
        [Url]
        public string UpstreamUrl { get; set; } = string.Empty;

        /// The HTTP methods supported by the API.
        public List<string>? Methods { get; set; }

        /// The hosts for the API.
        public List<string>? Hosts { get; set; }

        /// The tags for grouping the API.
        public List<string>? Tags { get; set; }

        /// Indicates whether the API is enabled.
        public bool IsEnabled { get; set; } = true;

        /// The ID of the product that the API is associated with.
        [Required]
        public Guid ApiProductId { get; set; }
    }
}
