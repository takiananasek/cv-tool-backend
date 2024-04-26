using CVTool.Data.Model;
using CVTool.Models.Common;

namespace CVTool.Models.AddResume
{
    public class AddResumeRequestDTO
    {
        public string Title { get; set; }
        public int? OwnerId { get; set; }
        public string? ProfileImageMetadataName { get; set; }
        public string? BackgroundImageMetadataName { get; set; }
        public ICollection<ComponentDTO> Components { get; set; }
    }
}
