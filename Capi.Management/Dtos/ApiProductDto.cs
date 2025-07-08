using System;
using System.Collections.Generic;

namespace Capi.Management.Dtos
{
    public class ApiProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ApiCount { get; set; }
    }
}
