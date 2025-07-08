using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Dtos
{
    public class ApiProductCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
