using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Models
{
    public class ApiProduct
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for the many-to-many relationship
        public virtual ICollection<Api> Apis { get; set; } = new List<Api>();
    }
}
