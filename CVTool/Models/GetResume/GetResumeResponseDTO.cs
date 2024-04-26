using CVTool.Models.Common;

namespace CVTool.Models.GetResume
{
    public class GetResumeResponseDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? OwnerId { get; set; }
        public string? ProfileImageMetadataName { get; set; }
        public string? BackgroundImageMetadataName { get; set; }
        public ICollection<ComponentDTO>? Components { get; set; }
    }
}
