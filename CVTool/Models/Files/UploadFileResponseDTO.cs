using System.Net;

namespace CVTool.Models.Files
{
    public class UploadFileResponseDTO
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string? Key { get; set; }
    }
}
