using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Dtos
{
 /// Data transfer object for a policy.
    public class PolicyDto
    {
         /// The type of the policy (e.g., "rate-limiting", "authentication").
        [Required]
        public string Type { get; set; } = string.Empty;

         /// The configuration for the policy, stored as a JSON string.
        [Required]
        public string Configuration { get; set; } = string.Empty;
    }
}
