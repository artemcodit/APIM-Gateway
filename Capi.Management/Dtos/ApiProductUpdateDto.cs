using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Dtos
{
    public class ApiProductUpdateDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
