using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Dtos
{
 /// Data transfer object for creating a new API.
    public class ApiCreateDto
    {
         /// The name of the API.
        [Required]
        public string Name { get; set; } = string.Empty;

         /// The route (path) for the API.
        [Required]
        public string Route { get; set; } = string.Empty;

         /// The upstream URL for the API.
        [Required]
        public string UpstreamUrl { get; set; } = string.Empty;

         /// The HTTP methods supported by the API.
        [Required]
        public List<string> Methods { get; set; } = new List<string>();
    }
}
