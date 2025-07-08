using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Models
{
    // Represents an API entity in the database.
    public class Api
    {
        // The unique identifier for the API.
        [Key]
        public Guid Id { get; set; }
        // The name of the API. It is required and has a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        // The route path for the API. It is required and has a maximum length of 200 characters.
        [Required]
        [StringLength(200)]
        public string Route { get; set; } = string.Empty;
        // The upstream URL for the API. It is required and has a maximum length of 500 characters.
        [Required]
        [StringLength(500)]
        public string UpstreamUrl { get; set; } = string.Empty;
        // The HTTP methods supported by the API.
        public List<string> Methods { get; set; } = new List<string>();
        // The hosts for the API.
        public List<string> Hosts { get; set; } = new List<string>();
        // The tags for grouping the API.
        public List<string> Tags { get; set; } = new List<string>();
        // The OpenAPI specification for the API.
        public string? OpenApiSpec { get; set; }
        // Indicates whether the API is enabled.
        public bool IsEnabled { get; set; } = true;
        // The policies associated with the API.
        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        // Navigation property for the many-to-many relationship
        public virtual ICollection<ApiProduct> ApiProducts { get; set; } = new List<ApiProduct>();
    }
}
