namespace Capi.Management.Dtos
{
    public class KongApiDto
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string[] Paths { get; set; } = Array.Empty<string>();
        public string[] Methods { get; set; } = Array.Empty<string>();
        public string[] Hosts { get; set; } = Array.Empty<string>();
    }
}
