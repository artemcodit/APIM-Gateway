using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Models
{
 /// Represents a policy to be applied to an API.
    public class Policy
    {
         /// The unique identifier for the policy.
        [Key]
        public Guid Id { get; set; }

         /// The type of the policy (e.g., "rate-limiting", "authentication").
        [Required]
        public string Type { get; set; } = string.Empty;

         /// The configuration for the policy, stored as a JSON string.
        [Required]
        public string Configuration { get; set; } = string.Empty; // JSON string for policy config

         /// The foreign key for the API this policy belongs to.
        public Guid ApiId { get; set; }

         /// The navigation property for the API this policy belongs to.
        public Api? Api { get; set; }
    }
}
