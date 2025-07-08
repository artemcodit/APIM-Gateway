using System.Collections.Generic;

namespace Capi.Management.Dtos
{
    public class ApiTestRequestDto
    {
        public string Method { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string? Body { get; set; }
    }
}
