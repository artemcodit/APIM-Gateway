using System.ComponentModel.DataAnnotations;

namespace Capi.Management.Dtos
{
    public class ApiUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Url]
        public string UpstreamUrl { get; set; } = string.Empty;

        [Required]
        public string Route { get; set; } = string.Empty;

        public bool IsEnabled { get; set; }

        public List<string>? Methods { get; set; }
        public List<string>? Hosts { get; set; }
        public List<string>? Tags { get; set; }

        public Guid? ApiProductId { get; set; }
    }
}
