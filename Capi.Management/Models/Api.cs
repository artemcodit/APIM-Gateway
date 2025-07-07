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
        // The HTTP methods supported by the API. It is a required list of strings.
        [Required]
        public List<string> Methods { get; set; } = new List<string>();
        // The list of policies associated with the API.
        public List<Policy> Policies { get; set; } = new List<Policy>();
    }
}
