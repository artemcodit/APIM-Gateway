using System.Collections.Generic;

namespace Capi.Management.Dtos
{
 /// Data transfer object for an API.
    public class ApiDto
    {
         /// The unique identifier for the API.
        public System.Guid Id { get; set; }

         /// The name of the API.
        public string Name { get; set; } = string.Empty;

         /// The route (path) for the API.
        public string Route { get; set; } = string.Empty;

         /// The upstream URL for the API.
        public string UpstreamUrl { get; set; } = string.Empty;

         /// The HTTP methods supported by the API.
        public List<string> Methods { get; set; } = new List<string>();

         /// The policies associated with the API.
        public List<PolicyDto> Policies { get; set; } = new List<PolicyDto>();
    }
}
