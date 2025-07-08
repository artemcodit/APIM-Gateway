namespace Capi.Management.Dtos
{
    public class ApiTestResponseDto
    {
        public int StatusCode { get; set; }
        public Dictionary<string, IEnumerable<string>> Headers { get; set; } = new Dictionary<string, IEnumerable<string>>();
        public string? Body { get; set; }
    }
}
